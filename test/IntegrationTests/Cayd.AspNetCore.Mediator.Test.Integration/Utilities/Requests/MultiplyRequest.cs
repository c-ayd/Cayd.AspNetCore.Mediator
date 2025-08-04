using Cayd.AspNetCore.Mediator.Abstraction;
using Cayd.AspNetCore.Mediator.Test.Integration.Utilities.Responses;

namespace Cayd.AspNetCore.Mediator.Test.Integration.Utilities.Requests
{
    public class MultiplyRequest : IAsyncRequest<MultiplyResponse>
    {
        public int Value { get; set; }
        public int Multiply { get; set; }
    }
}
