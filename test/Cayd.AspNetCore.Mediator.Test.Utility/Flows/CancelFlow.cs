using Cayd.AspNetCore.Mediator.Flows;
using Cayd.AspNetCore.Mediator.Test.Utility.Requests;
using Cayd.AspNetCore.Mediator.Test.Utility.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace Cayd.AspNetCore.Mediator.Test.Utility.Flows
{
    public class CancelFlow : IMediatorFlow<CancelledRequest, CancelledResponse>
    {
        public async Task<CancelledResponse> InvokeAsync(CancelledRequest request, AsyncHandlerDelegate<CancelledResponse> next, CancellationToken cancellationToken)
        {
            await Task.Delay(10000, cancellationToken);
            return await next();
        }
    }
}
