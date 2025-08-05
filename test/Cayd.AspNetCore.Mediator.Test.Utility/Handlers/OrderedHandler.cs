using Cayd.AspNetCore.Mediator.Abstractions;
using Cayd.AspNetCore.Mediator.Test.Utility.Requests;
using Cayd.AspNetCore.Mediator.Test.Utility.Responses;
using Cayd.AspNetCore.Mediator.Test.Utility.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Cayd.AspNetCore.Mediator.Test.Utility.Handlers
{
    public class OrderedHandler : IAsyncHandler<OrderedRequest, OrderedResponse>
    {
        private readonly OrderedService _orderedService;

        public OrderedHandler(OrderedService orderedService)
        {
            _orderedService = orderedService;
        }

        public async Task<OrderedResponse> HandleAsync(OrderedRequest request, CancellationToken cancellationToken)
        {
            _orderedService.Value /= request.Value;

            return new OrderedResponse()
            {
                Result = _orderedService.Value
            };
        }
    }
}
