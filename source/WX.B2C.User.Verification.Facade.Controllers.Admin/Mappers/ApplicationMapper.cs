using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Enums;
using TaskVariantDto = WX.B2C.User.Verification.Core.Contracts.Dtos.Policy.TaskVariantDto;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers
{
    public interface IApplicationMapper
    {
        ApplicationDto Map(
            Core.Contracts.Dtos.ApplicationDto application,
            IEnumerable<TaskVariantDto> taskVariants,
            IEnumerable<ApplicationAction> allowedActions);
    }

    internal class ApplicationMapper : IApplicationMapper
    {
        public ApplicationDto Map(
            Core.Contracts.Dtos.ApplicationDto application,
            IEnumerable<TaskVariantDto> taskVariants,
            IEnumerable<ApplicationAction> allowedActions)
        {
            if (application == null)
                throw new ArgumentNullException(nameof(application));
            if (taskVariants == null)
                throw new ArgumentNullException(nameof(taskVariants));
            if (allowedActions == null)
                throw new ArgumentNullException(nameof(allowedActions));

            var requiredTasks = application.Tasks
                                           .Join(
                                               taskVariants,
                                               task => task.VariantId,
                                               variant => variant.VariantId,
                                               MapTask)
                                           .ToArray();

            return new ApplicationDto
            {
                Id = application.Id,
                State = application.State,
                RequiredTasks = requiredTasks,
                DecisionReasons = application.DecisionReasons,
                AllowedActions = allowedActions.ToArray(),
                IsAutomating = application.IsAutomating,
                CreatedAt = application.CreatedAt
            };
        }

        private static ApplicationTaskDto MapTask(
            Core.Contracts.Dtos.ApplicationTaskDto taskDto,
            TaskVariantDto variantDto)
        {
            if (taskDto == null)
                throw new ArgumentNullException(nameof(taskDto));
            if (variantDto == null)
                throw new ArgumentNullException(nameof(variantDto));

            return new ApplicationTaskDto
            {
                Id = taskDto.Id,
                Name = variantDto.TaskName,
                Priority = variantDto.Priority,
                State = taskDto.State,
                Result = taskDto.Result
            };
        }
    }
}