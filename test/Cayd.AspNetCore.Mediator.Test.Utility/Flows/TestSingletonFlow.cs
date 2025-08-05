using Cayd.AspNetCore.Mediator.Abstractions;
using Cayd.AspNetCore.Mediator.Flows;
using System.Threading.Tasks;
using System.Threading;
using Cayd.AspNetCore.Mediator.Test.Utility.Services;

namespace Cayd.AspNetCore.Mediator.Test.Utility.Flows
{
    public class TestSingletonFlow<TRequest, TResponse> : IMediatorFlow<TRequest, TResponse>
        where TRequest : IAsyncRequest<TResponse>
    {
        private readonly TestSingletonService _testSingletonService;

        public TestSingletonFlow(TestSingletonService testSingletonService)
        {
            _testSingletonService = testSingletonService;
        }

        public async Task<TResponse> InvokeAsync(TRequest request, AsyncHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _testSingletonService.Counter = 100;
            return await next();
        }
    }
}
