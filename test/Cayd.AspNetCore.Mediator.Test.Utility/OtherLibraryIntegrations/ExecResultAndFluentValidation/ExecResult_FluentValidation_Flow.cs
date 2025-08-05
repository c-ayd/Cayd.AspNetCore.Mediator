using Cayd.AspNetCore.ExecutionResult;
using Cayd.AspNetCore.ExecutionResult.ClientError;
using Cayd.AspNetCore.Mediator.Abstractions;
using Cayd.AspNetCore.Mediator.Flows;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cayd.AspNetCore.Mediator.Test.Utility.OtherLibraryIntegrations.ExecResultAndFluentValidation
{
    public class ExecResult_FluentValidation_Flow<TRequest, TResponse> : IMediatorFlow<TRequest, TResponse>
        where TRequest : IAsyncRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ExecResult_FluentValidation_Flow(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> InvokeAsync(TRequest request, AsyncHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            foreach (var validator in _validators)
            {
                if (validator == null)
                    continue;

                var validationResult = await validator.ValidateAsync(request, cancellationToken);
                if (validationResult.Errors.Count > 0)
                {
                    var errorDetails = validationResult.Errors
                        .Select(e => new ExecErrorDetail(e.ErrorMessage, e.ErrorCode))
                        .ToList();

                    return (dynamic)new ExecBadRequest(errorDetails);
                }
            }

            return await next();
        }
    }
}
