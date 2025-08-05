using Cayd.AspNetCore.Mediator.Abstractions;
using Cayd.AspNetCore.Mediator.Test.Utility.Responses;

namespace Cayd.AspNetCore.Mediator.Test.Utility.Requests
{
    public class AddRequest : IAsyncRequest<AddResponse>
    {
        public int Value { get; set; }
        public int Add { get; set; }
    }
}
