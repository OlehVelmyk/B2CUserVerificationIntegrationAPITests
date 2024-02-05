using System.Linq;
using FluentValidation;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Models;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Validators
{
    internal class ExternalIdValidator : AbstractValidator<UserConsistency>
    {
        public ExternalIdValidator()
        {
            RuleFor(user => user.PassFortProfileId)
                .NotEmpty()
                .WithErrorCode(ErrorCodes.AbsentPassFortProfile)
                .When(state => state.Tasks.FirstOrDefault(task => task.Type == TaskType.RiskListsScreening)?.State
                            == TaskState.Completed)
                .When(user => user.Region != Region.USA);

            RuleFor(user => user.OnfidoApplicationId)
                .NotEmpty()
                .WithErrorCode(ErrorCodes.AbsentOnfidoApplicantIdWhenApplicationWasApproved)
                .When(user => user.Application.State is ApplicationState.InReview or ApplicationState.Approved);

            RuleFor(user => user.OnfidoApplicationId)
                .NotEmpty()
                .WithErrorCode(ErrorCodes.AbsentOnfidoApplicantIdWhenIdentityTaskCompleted)
                .When(user => user.Tasks.FirstOrDefault(task => task.Type == TaskType.Identity)?.State == TaskState.Completed);
        }
    }
}