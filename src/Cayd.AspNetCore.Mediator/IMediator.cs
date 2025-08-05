using Cayd.AspNetCore.Mediator.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Cayd.AspNetCore.Mediator
{
    public interface IMediator
    {
        Task<TResponse> SendAsync<TResponse>(IAsyncRequest<TResponse> request, CancellationToken cancellationToken = default);
    }
}
