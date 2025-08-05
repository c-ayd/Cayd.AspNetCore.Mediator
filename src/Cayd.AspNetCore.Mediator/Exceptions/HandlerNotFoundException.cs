using System;

namespace Cayd.AspNetCore.Mediator.Exceptions
{
    public class HandlerNotFoundException : Exception
    {
        public HandlerNotFoundException(string requestName) 
            : base($"A handler for {requestName} could not be found. This might be due to the handler not being registered from the correct assembly.")
        {
        }
    }
}
