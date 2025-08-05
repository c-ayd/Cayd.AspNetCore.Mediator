using Cayd.AspNetCore.Mediator.Abstractions;
using Cayd.AspNetCore.Mediator.Test.Integration.OtherAssembly.Responses;

namespace Cayd.AspNetCore.Mediator.Test.Integration.OtherAssembly.Requests
{
    public class DivideOtherAssemblyRequest : IAsyncRequest<DivideOtherAssemblyResponse>
    {
        public int Value { get; set; }
        public int Divide { get; set; }
    }
}
