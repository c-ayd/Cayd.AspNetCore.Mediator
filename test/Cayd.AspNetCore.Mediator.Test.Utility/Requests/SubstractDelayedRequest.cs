using Cayd.AspNetCore.Mediator.Abstractions;
using Cayd.AspNetCore.Mediator.Test.Utility.Responses;

namespace Cayd.AspNetCore.Mediator.Test.Utility.Requests
{
    public class SubstractDelayedRequest : IAsyncRequest<SubstractDelayedResponse>
    {
        public int Value { get; set; }
        public int Substract { get; set; }
    }
}
