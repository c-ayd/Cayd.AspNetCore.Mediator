using Cayd.AspNetCore.Mediator.Abstractions;
using Cayd.AspNetCore.Mediator.Test.Utility.Responses;

namespace Cayd.AspNetCore.Mediator.Test.Utility.Requests
{
    public class OrderedRequest : IAsyncRequest<OrderedResponse>
    {
        public int Value { get; set; }
    }
}
