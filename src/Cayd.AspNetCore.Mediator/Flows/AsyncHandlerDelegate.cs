using System.Threading.Tasks;

namespace Cayd.AspNetCore.Mediator.Flows
{
    public delegate Task<TResponse> AsyncHandlerDelegate<TResponse>();
}
