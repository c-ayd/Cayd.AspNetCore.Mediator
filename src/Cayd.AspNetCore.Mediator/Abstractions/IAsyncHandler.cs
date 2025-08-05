using System.Threading;
using System.Threading.Tasks;

namespace Cayd.AspNetCore.Mediator.Abstractions
{
    public interface IAsyncHandler<TRequest, TResponse>
        where TRequest : IAsyncRequest<TResponse>
    {
        Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
    }
}
