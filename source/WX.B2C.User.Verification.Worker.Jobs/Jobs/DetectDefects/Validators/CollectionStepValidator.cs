using FluentValidation;
using WX.B2C.User.Verification.Domain.DataCollection;
using CollectionStep = WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Models.CollectionStep;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Validators
{
    internal class CollectionStepValidator : AbstractValidator<CollectionStep>
    {
        public CollectionStepValidator()
        {
            RuleFor(step => step.RelatedTasks)
                .NotEmpty()
                .WithErrorCode(ErrorCodes.StepCreatedButNotAttached)
                .WithState(step => step);

            RuleFor(step => step.State)
                .NotEqual(CollectionStepState.InReview)
                .When(step => !step.IsReviewRequired)
                .WithErrorCode(ErrorCodes.StepInReviewWhenReviewNotRequired)
                .WithState(step => step);

            RuleFor(step => step.Result)
                .NotNull()
                .When(step => step.IsReviewRequired && step.State == CollectionStepState.Completed)
                .WithErrorCode(ErrorCodes.AbsentReviewResultWhenCompletedStepRequiresReview)
                .WithState(step => step);

            RuleFor(step => step.Result)
                .Null()
                .When(step => !step.IsReviewRequired || step.State != CollectionStepState.Completed)
                .WithErrorCode(ErrorCodes.RudimentReviewResult)
                .WithState(step => step);
        }
    }
}