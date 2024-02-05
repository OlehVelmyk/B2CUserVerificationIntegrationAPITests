using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Models;
using Check = WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Models.Check;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Validators
{
    internal class TaskValidator : AbstractValidator<Task>
    {
        public TaskValidator(UserConsistency user)
        {
            var requiredSteps = user.CollectionSteps.Where(step => step.IsRequired).ToArray();
            var applicationId = user.Application.Id;
            var relatedChecks =
                new Func<Guid, IEnumerable<Check>>(taskId => user.Checks.Where(check => check.RelatedTasks.Contains(taskId)));

            RuleFor(task => task.ApplicationId)
                .Must(appId => appId.HasValue)
                .WithErrorCode(ErrorCodes.RudimentTaskCreatedWhenNoApplicationId)
                .WithState(task => task);

            RuleFor(task => task.ApplicationId)
                .Equal(applicationId)
                .WithErrorCode(ErrorCodes.RudimentTaskCreatedWhenWrongApplicationId)
                .When(task => task.ApplicationId.HasValue)
                .WithState(task => task);

            RuleFor(task => task.State)
                .Must(state => state == TaskState.Completed)
                .When(task => relatedChecks(task.Id).All(check => check.IsCompleted) &&
                              requiredSteps.Where(step => step.RelatedTasks.Contains(task.Id)).All(step => step.IsCompleted))
                .WithErrorCode(ErrorCodes.TaskIncompleteWhenNoBlockers)
                .WithState(task => task);

            RuleFor(task => task.State)
                .Must(state => state is TaskState.Incomplete)
                .When(task => relatedChecks(task.Id).Any(check => !check.IsCompleted))
                .WithErrorCode(ErrorCodes.TaskCompleteWhenCheckNotCompleted)
                .WithState(task => task);

            RuleFor(task => task.State)
                .Must(state => state is TaskState.Incomplete)
                .When(task => requiredSteps.Where(step => step.RelatedTasks.Contains(task.Id))
                                           .Any(step => !step.IsCompleted))
                .WithErrorCode(ErrorCodes.TaskCompleteWhenCollectionStepNotComplete)
                .WithState(task => task);

            // Specific task validations
            RuleFor(task => task.State)
                .Must(state => state is TaskState.Completed)
                .When(task => task.Type == TaskType.RiskListsScreening && !relatedChecks(task.Id).Any())
                .WithErrorCode(ErrorCodes.RiskListsScreeningIncompleteButNoChecks);

            RuleFor(task => task.State)
                .Must(state => state is TaskState.Completed)
                .When(task => task.Type == TaskType.Identity && !relatedChecks(task.Id).Any())
                .WithErrorCode(ErrorCodes.IdentityIncompleteButNoChecks);

            RuleFor(task => task.State)
                .Must(state => state is TaskState.Incomplete)
                .When(task => task.Type == TaskType.TaxResidence && !user.ProfileDataExistence.TaxResidence)
                .WithErrorCode(ErrorCodes.TaxResidenceCompletedWhenNoTaxResidence);
        }
    }
}