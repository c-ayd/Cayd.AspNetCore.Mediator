using Cayd.AspNetCore.Mediator.Abstraction;
using Cayd.AspNetCore.Mediator.Utilities;
using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Cayd.AspNetCore.Mediator
{
    public class Mediator : IMediator
    {
        private static readonly ConcurrentDictionary<Type, Func<IServiceProvider, object, CancellationToken, Task<object>>> _handlers = new();

        private readonly IServiceProvider _services;

        public Mediator(IServiceProvider services)
        {
            _services = services;
        }

        public async Task<TResponse> SendAsync<TResponse>(IAsyncRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var handle = _handlers.GetOrAdd(request.GetType(), (requestType) =>
            {
                var interfaceType = typeof(IAsyncRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));

                // Delegate parameters
                var servicesParameter = Expression.Parameter(typeof(IServiceProvider), "services");
                var requestParameter = Expression.Parameter(typeof(object), "request");
                var cancellationTokenParameter = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

                // (IAsyncRequestHandler<TRequest, TResponse>)services.GetService(interfaceType)
                var interfaceTypeConstant = Expression.Constant(interfaceType, typeof(Type));

                var getServiceMethod = typeof(IServiceProvider).GetMethod(nameof(IServiceProvider.GetService))!;
                var getServiceCall = Expression.Call(servicesParameter, getServiceMethod, interfaceTypeConstant);
                var handler = Expression.Convert(getServiceCall, interfaceType);

                // (IAsyncRequestHandler<TRequest, TResponse>)services.GetService(interfaceType)
                //     .HandleAsync(request, cancellationToken)
                var convertedRequestParameter = Expression.Convert(requestParameter, requestType);

                var handleAsyncMethod = interfaceType.GetMethod("HandleAsync")!;
                var handleAsyncCall = Expression.Call(handler, handleAsyncMethod, convertedRequestParameter, cancellationTokenParameter);
                var convertedHandleAsyncCall = Expression.Call(typeof(TaskConverter), nameof(TaskConverter.ConvertTask), new Type[] { typeof(TResponse) }, handleAsyncCall);

                // Convert it to lambda and then delegate
                var lambda = Expression.Lambda<Func<IServiceProvider, object, CancellationToken, Task<object>>>(convertedHandleAsyncCall,
                    servicesParameter,
                    requestParameter,
                    cancellationTokenParameter);

                return lambda.Compile();
            });

            var response = await handle(_services, request, cancellationToken);
            return (TResponse)response;
        }
    }

}
