using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Models;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Validators
{
    internal class TasksValidator : AbstractValidator<UserConsistency>
    {
        public TasksValidator()
        {
            RuleFor(user => user.Tasks)
                .Must(BeAttachedOnlyOneTypeOfTaskToApplication)
                .WithErrorCode(ErrorCodes.SeveralTaskCreatedWithOneType)
                .OverridePropertyName("ApplicationTasksTypes");

            RuleForEach(state => state.Tasks).SetValidator(state => new TaskValidator(state));
        }

        private bool BeAttachedOnlyOneTypeOfTaskToApplication(UserConsistency user, Task[] tasks, ValidationContext<UserConsistency> context)
        {
            var tasksAttachedToApplication = tasks.Where(task => task.ApplicationId == user.Application.Id).GroupBy(task => task.Type);
            var taskTypesWithDuplication = tasksAttachedToApplication.Where(grouping => grouping.Count() > 1)
                                                                     .Select(grouping => grouping.Key)
                                                                     .ToArray();

            if (taskTypesWithDuplication.Length == 0)
                return true;

            var validationFailure = new ValidationFailure("ApplicationTasks", "Several tasks created with one type in application")
            {
                ErrorCode = ErrorCodes.SeveralTaskCreatedWithOneType,
                CustomState = string.Join(",", taskTypesWithDuplication)
            };
            context.AddFailure(validationFailure);
            
            return true;
        }
    }
}