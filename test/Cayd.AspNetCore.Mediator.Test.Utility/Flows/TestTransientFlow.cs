using Cayd.AspNetCore.Mediator.Abstractions;
using Cayd.AspNetCore.Mediator.Flows;
using System.Threading.Tasks;
using System.Threading;
using Cayd.AspNetCore.Mediator.Test.Utility.Services;

namespace Cayd.AspNetCore.Mediator.Test.Utility.Flows
{
    public class TestTransientFlow<TRequest, TResponse> : IMediatorFlow<TRequest, TResponse>
        where TRequest : IAsyncRequest<TResponse>
    {
        private readonly TestScopedService _testScopedService;

        public TestTransientFlow(TestScopedService testScopedService)
        {
            _testScopedService = testScopedService;
        }

        public async Task<TResponse> InvokeAsync(TRequest request, AsyncHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            ++_testScopedService.Counter;
            return await next();
        }
    }
}
