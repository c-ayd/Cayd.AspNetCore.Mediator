using System.Threading;
using System.Threading.Tasks;

namespace Cayd.AspNetCore.Mediator.Abstractions
{
    /// <summary>
    /// Marks the class as a handler object for the corresponding <see cref="IAsyncRequest{TResponse}"/>.
    /// </summary>
    /// <typeparam name="TRequest">Type of <see cref="IAsyncRequest{TResponse}"/> that triggers this handler</typeparam>
    /// <typeparam name="TResponse">Returning type of this handler</typeparam>
    public interface IAsyncHandler<TRequest, TResponse>
        where TRequest : IAsyncRequest<TResponse>
    {
        /// <summary>
        /// It is called when the corresponding <see cref="IAsyncRequest{TResponse}"/> is sent.
        /// </summary>
        /// <param name="request">Corresponding <see cref="IAsyncRequest{TResponse}"/></param>
        /// <param name="cancellationToken">Notification of cancellation</param>
        /// <returns>Returns the handler's response.</returns>
        Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
    }
}
