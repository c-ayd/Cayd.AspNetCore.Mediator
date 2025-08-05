using Cayd.AspNetCore.Mediator.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Cayd.AspNetCore.Mediator.Flows
{
    public interface IMediatorFlow<TRequest, TResponse>
        where TRequest : IAsyncRequest<TResponse>
    {
        Task<TResponse> InvokeAsync(TRequest request, AsyncHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
    }
}
