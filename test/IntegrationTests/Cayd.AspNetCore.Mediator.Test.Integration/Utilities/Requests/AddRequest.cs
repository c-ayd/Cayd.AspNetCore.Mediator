using Cayd.AspNetCore.Mediator.Abstraction;
using Cayd.AspNetCore.Mediator.Test.Integration.Utilities.Responses;

namespace Cayd.AspNetCore.Mediator.Test.Integration.Utilities.Requests
{
    public class AddRequest : IAsyncRequest<AddResponse>
    {
        public int Value { get; set; }
        public int Add { get; set; }
    }
}
