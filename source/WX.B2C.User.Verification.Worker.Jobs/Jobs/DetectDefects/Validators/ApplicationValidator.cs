using System.Linq;
using FluentValidation;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Models;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Validators
{
    internal class ApplicationValidator : AbstractValidator<UserConsistency>
    {
        public ApplicationValidator()
        {
            RuleFor(user => user.Application.State)
                .Must(state => state == ApplicationState.Approved)
                .When(user => user.Tasks.All(task => task.State == TaskState.Completed && task.Result != TaskResult.Failed))
                .WithErrorCode(ErrorCodes.NotApprovedWhenAllTaskCompleted);

            RuleFor(user => user.Application.State)
                .Must(state => state is not ApplicationState.Approved)
                .When(user => user.Tasks.Any(task => task.State != TaskState.Completed || task.Result == TaskResult.Failed))
                .WithErrorCode(ErrorCodes.ApprovedWhenTaskIncompleteOrFailed);

            RuleFor(user => user.Application.State)
                .Must(state => state is not ApplicationState.Approved and not ApplicationState.InReview)
                .When(user => !user.ProfileDataExistence.RiskLevel)
                .WithErrorCode(ErrorCodes.ApprovedOrInReviewWhenNoRiskLevel);

            RuleFor(user => user.Region)
                .NotEqual(Region.Undefined)
                .WithErrorCode(ErrorCodes.ApplicationPolicyIsNotDefined)
                .WithState(consistency => consistency.Application.Id);
        }
    }
}