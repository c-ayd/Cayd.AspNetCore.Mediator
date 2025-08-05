using FluentValidation;

namespace Cayd.AspNetCore.Mediator.Test.Utility.OtherLibraryIntegrations.ExecResultAndFluentValidation
{
    public class ExecResult_FluentValidation_Validation : AbstractValidator<ExecResult_FluentValidation_Request>
    {
        public ExecResult_FluentValidation_Validation()
        {
            RuleFor(_ => _.Divide)
                .NotEqual(0)
                    .WithMessage("test error message")
                    .WithErrorCode("test error code");
        }
    }
}
