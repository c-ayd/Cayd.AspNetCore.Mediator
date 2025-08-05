using Cayd.AspNetCore.Mediator.Abstractions;
using Cayd.AspNetCore.Mediator.Test.Utility.Requests;
using Cayd.AspNetCore.Mediator.Test.Utility.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace Cayd.AspNetCore.Mediator.Test.Utility.Handlers
{
    public class CancelledHandler : IAsyncHandler<CancelledRequest, CancelledResponse>
    {
        public static int Value = 0;

        public async Task<CancelledResponse> HandleAsync(CancelledRequest request, CancellationToken cancellationToken)
        {
            Value = 100;
            return new CancelledResponse();
        }
    }
}
