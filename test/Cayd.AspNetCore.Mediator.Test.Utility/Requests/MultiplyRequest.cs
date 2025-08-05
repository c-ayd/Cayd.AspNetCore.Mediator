using Cayd.AspNetCore.Mediator.Abstractions;
using Cayd.AspNetCore.Mediator.Test.Utility.Responses;

namespace Cayd.AspNetCore.Mediator.Test.Utility.Requests
{
    public class MultiplyRequest : IAsyncRequest<MultiplyResponse>
    {
        public int Value { get; set; }
        public int Multiply { get; set; }
    }
}
