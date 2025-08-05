using Cayd.AspNetCore.Mediator.Flows;
using System;

namespace Cayd.AspNetCore.Mediator.Exceptions
{
    public class WrongTypeException : Exception
    {
        public WrongTypeException(string flowTypeName)
            : base($"{flowTypeName} must be a non-abstract class implementing {typeof(IMediatorFlow<,>)}.")
        {
        }
    }
}
