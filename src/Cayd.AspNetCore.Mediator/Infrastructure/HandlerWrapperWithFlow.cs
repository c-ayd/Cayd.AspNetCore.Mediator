using Cayd.AspNetCore.Mediator.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cayd.AspNetCore.Mediator.Infrastructure
{
    public class HandlerWrapperWithFlow : HandlerWrapperBase
    {
        private readonly Func<IServiceProvider, object> _getHandlerDelegate;
        private readonly Func<IServiceProvider, List<object>> _getMediatorFlowsDelegate;
        private readonly Func<object, object, CancellationToken, object> _setInitialHandlerDelegateDelegate;
        private readonly Func<object, object, object, CancellationToken, object> _loopBodyDelegate;
        private readonly Func<object, Task<object>> _callFirstDelegateDelegate;

        public HandlerWrapperWithFlow(Func<IServiceProvider, object> getHandlerDelegate,
            Func<IServiceProvider, List<object>> getMediatorFlowsDelegate,
            Func<object, object, CancellationToken, object> setInitialHandlerDelegateDelegate,
            Func<object, object, object, CancellationToken, object> loopBodyDelegate,
            Func<object, Task<object>> callFirstDelegateDelegate)
        {
            _getHandlerDelegate = getHandlerDelegate;
            _getMediatorFlowsDelegate = getMediatorFlowsDelegate;
            _setInitialHandlerDelegateDelegate = setInitialHandlerDelegateDelegate;
            _loopBodyDelegate = loopBodyDelegate;
            _callFirstDelegateDelegate = callFirstDelegateDelegate;
        }

        public override async Task<TResponse> Handle<TResponse>(IServiceProvider services, object request, CancellationToken cancellationToken)
        {
            var handler = _getHandlerDelegate(services);
            if (handler == null)
                throw new HandlerNotFoundException(request.GetType().Name);

            var flows = _getMediatorFlowsDelegate(services);

            var handlerDelegate = _setInitialHandlerDelegateDelegate(handler, request, cancellationToken);

            for (int i = flows.Count - 1; i >= 0; --i)
            {
                handlerDelegate = _loopBodyDelegate(handlerDelegate, flows[i], request, cancellationToken);
            }

            var response = await _callFirstDelegateDelegate(handlerDelegate);
            return (TResponse)response;
        }
    }
}
