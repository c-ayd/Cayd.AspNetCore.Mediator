using Cayd.AspNetCore.Mediator.Abstractions;
using System.Threading.Tasks;

namespace Cayd.AspNetCore.Mediator.Flows
{
    /// <summary>
    /// The next delegate in the current request-handler-response flow.
    /// </summary>
    /// <typeparam name="TResponse">Return type of the corresponding <see cref="IAsyncHandler{TRequest, TResponse}"/></typeparam>
    /// <returns>Returns the response of the corresponding <see cref="IAsyncHandler{TRequest, TResponse}"/>.</returns>
    public delegate Task<TResponse> AsyncHandlerDelegate<TResponse>();
}
