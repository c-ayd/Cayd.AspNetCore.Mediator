using Cayd.AspNetCore.Mediator.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cayd.AspNetCore.Mediator.Infrastructure
{
    public class HandlerWrapperWithoutFlow : HandlerWrapperBase
    {
        private readonly Func<IServiceProvider, object> _getHandlerDelegate;
        private readonly Func<object, object, CancellationToken, Task<object>> _callHandlerDelegate;

        public HandlerWrapperWithoutFlow(Func<IServiceProvider, object> getHandlerDelegate,
            Func<object, object, CancellationToken, Task<object>> callHandlerDelegate)
        {
            _getHandlerDelegate = getHandlerDelegate;
            _callHandlerDelegate = callHandlerDelegate;
        }

        public override async Task<TResponse> Handle<TResponse>(IServiceProvider services, object request, CancellationToken cancellationToken)
        {
            var handler = _getHandlerDelegate(services);
            if (handler == null)
                throw new HandlerNotFoundException(request.GetType().Name);

            var response = await _callHandlerDelegate(handler, request, cancellationToken);
            return (TResponse)response;
        }
    }
}
