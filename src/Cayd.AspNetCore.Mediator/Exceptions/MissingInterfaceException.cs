using Cayd.AspNetCore.Mediator.Flows;
using System;

namespace Cayd.AspNetCore.Mediator.Exceptions
{
    public class MissingInterfaceException : Exception
    {
        public MissingInterfaceException(string flowTypeName)
            : base($"{flowTypeName} does not implement {typeof(IMediatorFlow<,>).Name}.")
        {
        }
    }
}
