using Cayd.AspNetCore.Mediator.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Cayd.AspNetCore.Mediator
{
    /// <summary>
    /// Handles request-handler-response flows.
    /// </summary>
    public interface IMediator
    {
        /// <summary>
        /// Sends <paramref name="request"/> to start the corresponding request-handler-response flow.
        /// </summary>
        /// <typeparam name="TResponse">Response type of the corresponding <see cref="IAsyncHandler{TRequest, TResponse}"/></typeparam>
        /// <param name="request">Request to start request-handler-response flow</param>
        /// <param name="cancellationToken">Notification of cancellation</param>
        /// <returns>Returns the response of the corresponding <see cref="IAsyncHandler{TRequest, TResponse}"/>.</returns>
        Task<TResponse> SendAsync<TResponse>(IAsyncRequest<TResponse> request, CancellationToken cancellationToken = default);
    }
}
