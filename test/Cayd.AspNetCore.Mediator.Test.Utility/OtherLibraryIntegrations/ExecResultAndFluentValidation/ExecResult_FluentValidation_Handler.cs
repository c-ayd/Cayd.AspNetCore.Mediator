using Cayd.AspNetCore.ExecutionResult;
using Cayd.AspNetCore.Mediator.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Cayd.AspNetCore.Mediator.Test.Utility.OtherLibraryIntegrations.ExecResultAndFluentValidation
{
    public class ExecResult_FluentValidation_Handler : IAsyncHandler<ExecResult_FluentValidation_Request, ExecResult<ExecResult_FluentValidation_Response>>
    {
        public async Task<ExecResult<ExecResult_FluentValidation_Response>> HandleAsync(ExecResult_FluentValidation_Request request, CancellationToken cancellationToken)
        {
            return new ExecResult_FluentValidation_Response()
            {
                Result = 10 / request.Divide
            };
        }
    }
}
