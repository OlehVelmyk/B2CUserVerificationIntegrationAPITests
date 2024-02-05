using FluentValidation;
using WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Models;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Validators
{
    internal class UserConsistencyValidator : AbstractValidator<UserConsistency>
    {
        private readonly ApplicationValidator _applicationValidator = new();
        private readonly DataPersistenceValidator _dataPersistenceValidator = new();
        private readonly ExternalIdValidator _externalIdValidator = new();
        private readonly TasksValidator _tasksValidator = new();
        private readonly CollectionStepsValidator _collectionStepsValidator = new();

        public UserConsistencyValidator()
        {
            RuleFor(user => user.Application).NotNull().WithErrorCode(ErrorCodes.NoApplication);
            
            RuleFor(user => user)
                .SetValidator(_applicationValidator)
                .When(user => user.Application != null)
                .WithState(consistency => consistency.Application.Id);

            RuleFor(user => user).SetValidator(_dataPersistenceValidator).When(user => user.Application != null);
            RuleFor(user => user).SetValidator(_externalIdValidator).When(user => user.Application != null);
            RuleFor(user => user).SetValidator(_tasksValidator).When(user => user.Application != null);
            RuleFor(user => user).SetValidator(_collectionStepsValidator).When(user => user.Application != null);
        }
    }
}