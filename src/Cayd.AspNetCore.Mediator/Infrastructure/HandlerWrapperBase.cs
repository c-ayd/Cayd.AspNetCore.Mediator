using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cayd.AspNetCore.Mediator.Infrastructure
{
    public abstract class HandlerWrapperBase
    {
        public abstract Task<TResponse> Handle<TResponse>(IServiceProvider services, object request, CancellationToken cancellationToken);
    }
}
