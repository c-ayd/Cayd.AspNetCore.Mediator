using Cayd.AspNetCore.Mediator.Abstractions;
using Cayd.AspNetCore.Mediator.Test.Integration.OtherAssembly.Requests;
using Cayd.AspNetCore.Mediator.Test.Integration.OtherAssembly.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace Cayd.AspNetCore.Mediator.Test.Integration.OtherAssembly.Handlers
{
    public class DivideOtherAssemblyHandler : IAsyncHandler<DivideOtherAssemblyRequest, DivideOtherAssemblyResponse>
    {
        public async Task<DivideOtherAssemblyResponse> HandleAsync(DivideOtherAssemblyRequest request, CancellationToken cancellationToken = default)
        {
            return new DivideOtherAssemblyResponse()
            {
                Result = request.Value / request.Divide
            };
        }
    }
}
