using Cayd.AspNetCore.Mediator.Abstractions;
using Cayd.AspNetCore.Mediator.Flows;
using Cayd.AspNetCore.Mediator.Test.Utility.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Cayd.AspNetCore.Mediator.Test.Utility.Flows
{
    public class TestScopedFlow<TRequest, TResponse> : IMediatorFlow<TRequest, TResponse>
        where TRequest : IAsyncRequest<TResponse>
    {
        private readonly TestScopedService _testScopedService;

        public TestScopedFlow(TestScopedService testScopedService)
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
