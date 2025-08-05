using Cayd.AspNetCore.Mediator.Flows;
using Cayd.AspNetCore.Mediator.Test.Utility.Requests;
using Cayd.AspNetCore.Mediator.Test.Utility.Responses;
using Cayd.AspNetCore.Mediator.Test.Utility.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Cayd.AspNetCore.Mediator.Test.Utility.Flows
{
    public class Ordered1Flow : IMediatorFlow<OrderedRequest, OrderedResponse>
    {
        private readonly OrderedService _orderedService;
        private readonly TestSingletonService _testSingletonService;

        public Ordered1Flow(OrderedService orderedService,
            TestSingletonService testSingletonService)
        {
            _orderedService = orderedService;
            _testSingletonService = testSingletonService;
        }

        public async Task<OrderedResponse> InvokeAsync(OrderedRequest request, AsyncHandlerDelegate<OrderedResponse> next, CancellationToken cancellationToken)
        {
            _orderedService.Value *= 2;
            _testSingletonService.Counter = 1_000_000;
            return await next();
        }
    }
}
