using Cayd.AspNetCore.ExecutionResult;
using Cayd.AspNetCore.Mediator.Abstractions;

namespace Cayd.AspNetCore.Mediator.Test.Utility.OtherLibraryIntegrations.ExecResultAndFluentValidation
{
    public class ExecResult_FluentValidation_Request : IAsyncRequest<ExecResult<ExecResult_FluentValidation_Response>>
    {
        public int Divide { get; set; }
    }
}
