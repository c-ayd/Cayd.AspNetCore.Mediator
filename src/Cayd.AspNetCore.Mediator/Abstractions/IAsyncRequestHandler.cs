using System.Threading;
using System.Threading.Tasks;

namespace Cayd.AspNetCore.Mediator.Abstraction
{
    public interface IAsyncRequestHandler<TRequest, TResponse>
        where TRequest : IAsyncRequest<TResponse>
    {
        Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
    }
}
