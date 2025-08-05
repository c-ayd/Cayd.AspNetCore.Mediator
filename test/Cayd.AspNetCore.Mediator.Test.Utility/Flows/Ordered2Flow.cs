using System.Threading.Tasks;
using System.Threading;
using Cayd.AspNetCore.Mediator.Test.Utility.Services;
using Cayd.AspNetCore.Mediator.Test.Utility.Responses;
using Cayd.AspNetCore.Mediator.Test.Utility.Requests;
using Cayd.AspNetCore.Mediator.Flows;

namespace Cayd.AspNetCore.Mediator.Test.Utility.Flows
{
    public class Ordered2Flow : IMediatorFlow<OrderedRequest, OrderedResponse>
    {
        private readonly OrderedService _orderedService;

        public Ordered2Flow(OrderedService orderedService)
        {
            _orderedService = orderedService;
        }

        public async Task<OrderedResponse> InvokeAsync(OrderedRequest request, AsyncHandlerDelegate<OrderedResponse> next, CancellationToken cancellationToken)
        {
            _orderedService.Value += 5;
            return await next();
        }
    }
}
