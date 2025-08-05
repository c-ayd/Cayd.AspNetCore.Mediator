using Cayd.AspNetCore.Mediator.Abstractions;
using Cayd.AspNetCore.Mediator.Infrastructure;
using Cayd.AspNetCore.Mediator.Flows;
using Cayd.AspNetCore.Mediator.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Cayd.AspNetCore.Mediator
{
    public class Mediator : IMediator
    {
        private static readonly string _handlerMethodName = "HandleAsync";
        private static readonly string _mediatorFlowMethodName = "InvokeAsync";

        private static readonly ConcurrentDictionary<Type, HandlerWrapperBase> _handlerWrappers = new();
        private static bool IsThereAnyMediatorFlow = false;
        
        private readonly IServiceProvider _services;

        public Mediator(IServiceProvider services)
        {
            _services = services;
        }

        public async Task<TResponse> SendAsync<TResponse>(IAsyncRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var wrapper = _handlerWrappers.GetOrAdd(request.GetType(), (requestType) =>
                IsThereAnyMediatorFlow ? GetHandlerWrapperWithMediatorFlows<TResponse>(requestType) : GetHandlerWrapperWithoutMediatorFlow<TResponse>(requestType));

            return await wrapper.Handle<TResponse>(_services, request, cancellationToken);
        }

        private HandlerWrapperBase GetHandlerWrapperWithMediatorFlows<TResponse>(Type requestType)
        {
            var responseType = typeof(TResponse);

            var handlerInterfaceType = typeof(IAsyncHandler<,>).MakeGenericType(requestType, responseType);
            var flowInterfaceType = typeof(IMediatorFlow<,>).MakeGenericType(requestType, responseType);

            // Delegate parameters
            var servicesParameter = Expression.Parameter(typeof(IServiceProvider), "services");
            var requestParameter = Expression.Parameter(typeof(object), "request");
            var cancellationTokenParameter = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

            // (TRequest)request;
            var convertedRequestParameter = Expression.Convert(requestParameter, requestType);

            /**
             * services.GetService(typeof(IAsyncHandler<TRequest, TResponse>));
             */
            var getServiceMethod = typeof(IServiceProvider).GetMethod(nameof(IServiceProvider.GetService))!;
            var getServiceCall = Expression.Call(servicesParameter, getServiceMethod,
                Expression.Constant(handlerInterfaceType, typeof(Type)));

            var getServiceCallLambda = Expression.Lambda<Func<IServiceProvider, object>>(getServiceCall,
                servicesParameter);

            /**
             * services.GetServices(typeof(IMediatorFlow<TRequest, TResponse>))
             *     .ToList<object>();
             */
            var getServicesMethod = typeof(ServiceProviderServiceExtensions).GetMethods()
                .First(m => !m.IsGenericMethod && m.Name == nameof(ServiceProviderServiceExtensions.GetServices));
            var getServicesCall = Expression.Call(getServicesMethod,
                servicesParameter,
                Expression.Constant(flowInterfaceType, typeof(Type)));

            var toListMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList))!.MakeGenericMethod(typeof(object));
            var toListCall = Expression.Call(toListMethod,
                Expression.Convert(getServicesCall, typeof(IEnumerable<object>)));

            var toListCallLambda = Expression.Lambda<Func<IServiceProvider, List<object>>>(toListCall,
                servicesParameter);

            /**
             * () => (IMediatorFlow<TRequest, TResponse>)handler.HandleAsync((TRequest)request, cancellationToken);
             */
            var handlerParameter = Expression.Parameter(typeof(object), "handler");

            var convertedHandlerParameter = Expression.Convert(handlerParameter, handlerInterfaceType);
            var handleAsyncMethod = handlerInterfaceType.GetMethod(_handlerMethodName)!;
            var handleAsyncCall = Expression.Call(convertedHandlerParameter, handleAsyncMethod,
                convertedRequestParameter,
                cancellationTokenParameter);

            var handlerDelegateType = typeof(AsyncHandlerDelegate<>).MakeGenericType(responseType);
            var handlerDelegateLambda = Expression.Lambda(handlerDelegateType, handleAsyncCall);

            var convertedHandlerDelegateLambda = Expression.Convert(handlerDelegateLambda, typeof(object));
            var convertedHandlerDelegateLambdaLambda = Expression.Lambda<Func<object, object, CancellationToken, object>>(convertedHandlerDelegateLambda,
                handlerParameter,
                requestParameter,
                cancellationTokenParameter);

            /**
             * () => (IMediatorFlow<TRequest, TResponse>)currentFlow.InvokeAsync((TRequest)request, (AsyncHandlerDelegate<TRequest, TResponse>)handlerDelegate, cancellationToken);
             */
            var handlerDelegateParameter = Expression.Parameter(typeof(object), "handlerDelegate");
            var currentFlowParameter = Expression.Parameter(typeof(object), "currentFlow");

            var convertedHandlerDelegateParameter = Expression.Convert(handlerDelegateParameter, handlerDelegateType);
            var convertedCurrentFlowParameter = Expression.Convert(currentFlowParameter, flowInterfaceType);

            var invokeAsyncMethod = flowInterfaceType.GetMethod(_mediatorFlowMethodName)!;
            var invokeAsyncCall = Expression.Call(convertedCurrentFlowParameter, invokeAsyncMethod,
                convertedRequestParameter,
                convertedHandlerDelegateParameter,
                cancellationTokenParameter);
            var invokeAsyncCallLambda = Expression.Lambda(handlerDelegateType, invokeAsyncCall);

            var convertedInvokeAsyncCallLambda = Expression.Convert(invokeAsyncCallLambda, typeof(object));
            var convertedInvokeAsyncCallLambdaLambda = Expression.Lambda<Func<object, object, object, CancellationToken, object>>(convertedInvokeAsyncCallLambda,
                handlerDelegateParameter,
                currentFlowParameter,
                requestParameter,
                cancellationTokenParameter);

            /**
             * (IAsyncHandler<TRequest, TResponse>)handlerDelegate((TRequest)request, cancellationToken)
             */
            var handlerDelegateInvoke = Expression.Invoke(convertedHandlerDelegateParameter);
            var convertedHandlerDelegateInvoke = Expression.Call(typeof(TaskConverter), nameof(TaskConverter.Convert), new Type[] { responseType },
                handlerDelegateInvoke);

            var convertedHandlerDelegateInvokeLambda = Expression.Lambda<Func<object, Task<object>>>(convertedHandlerDelegateInvoke,
                handlerDelegateParameter);

            return new HandlerWrapperWithFlow(getServiceCallLambda.Compile(),
                toListCallLambda.Compile(),
                convertedHandlerDelegateLambdaLambda.Compile(),
                convertedInvokeAsyncCallLambdaLambda.Compile(),
                convertedHandlerDelegateInvokeLambda.Compile());
        }

        private HandlerWrapperBase GetHandlerWrapperWithoutMediatorFlow<TResponse>(Type requestType)
        {
            var responseType = typeof(TResponse);

            var handlerInterfaceType = typeof(IAsyncHandler<,>).MakeGenericType(requestType, responseType);

            // Delegate parameters
            var servicesParameter = Expression.Parameter(typeof(IServiceProvider), "services");
            var requestParameter = Expression.Parameter(typeof(object), "request");
            var cancellationTokenParameter = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

            // (TRequest)request;
            var convertedRequestParameter = Expression.Convert(requestParameter, requestType);

            /**
             * services.GetService(typeof(IAsyncHandler<TRequest, TResponse>));
             */
            var getServiceMethod = typeof(IServiceProvider).GetMethod(nameof(IServiceProvider.GetService))!;
            var getServiceCall = Expression.Call(servicesParameter, getServiceMethod,
                Expression.Constant(handlerInterfaceType, typeof(Type)));

            var getServiceCallLambda = Expression.Lambda<Func<IServiceProvider, object>>(getServiceCall,
                servicesParameter);

            /**
             * (IAsyncHandler<TRequest, TResponse>)handler.HandleAsync((TRequest)request, cancellationToken);
             */
            var handlerParameter = Expression.Parameter(typeof(object), "handler");

            var convertedHandlerParameter = Expression.Convert(handlerParameter, handlerInterfaceType);
            var handlerAsyncMethod = handlerInterfaceType.GetMethod(_handlerMethodName)!;
            var handlerAsyncCall = Expression.Call(convertedHandlerParameter, handlerAsyncMethod,
                convertedRequestParameter,
                cancellationTokenParameter);

            var convertedHandlerAsyncCall = Expression.Call(typeof(TaskConverter), nameof(TaskConverter.Convert), new Type[] { responseType },
                handlerAsyncCall);
            var convertedHandlerAsyncCallLambda = Expression.Lambda<Func<object, object, CancellationToken, Task<object>>>(convertedHandlerAsyncCall,
                handlerParameter,
                requestParameter,
                cancellationTokenParameter);

            return new HandlerWrapperWithoutFlow(getServiceCallLambda.Compile(),
                convertedHandlerAsyncCallLambda.Compile());
        }
    }

}
