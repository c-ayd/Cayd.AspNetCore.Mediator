using Cayd.AspNetCore.Mediator.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Cayd.AspNetCore.Mediator.Flows
{
    /// <summary>
    /// Marks the class as a custom flow element for <see cref="IMediator"/>.
    /// <para>
    /// If the flow element is generic then it is applied to all request.
    /// Otherwise, it is applied to defined <typeparamref name="TRequest"/> and <typeparamref name="TResponse"/>.
    /// </para>
    /// </summary>
    /// <typeparam name="TRequest">Type of <see cref="IAsyncRequest{TResponse}"/> to which this flow element is added</typeparam>
    /// <typeparam name="TResponse">Response type of the corresponding <see cref="IAsyncHandler{TRequest, TResponse}"/></typeparam>
    public interface IMediatorFlow<TRequest, TResponse>
        where TRequest : IAsyncRequest<TResponse>
    {
        /// <summary>
        /// It is called during the corresponding the request-handler-response flow.
        /// </summary>
        /// <param name="request">Request element triggering this flow element</param>
        /// <param name="next">Next custom flow element or the corresponding <see cref="IAsyncHandler{TRequest, TResponse}"/> in the current flow</param>
        /// <param name="cancellationToken">Notification of cancellation</param>
        /// <returns>Returns the response of the corresponding <see cref="IAsyncHandler{TRequest, TResponse}"/>.</returns>
        Task<TResponse> InvokeAsync(TRequest request, AsyncHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
    }
}
