namespace Cayd.AspNetCore.Mediator.Abstractions
{
    /// <summary>
    /// Marks the class as a request object for the corresponding <see cref="IAsyncHandler{TRequest, TResponse}"/>.
    /// </summary>
    /// <typeparam name="TResponse">Response type of the corresponding <see cref="IAsyncHandler{TRequest, TResponse}"/></typeparam>
    public interface IAsyncRequest<TResponse>
    {
    }
}
