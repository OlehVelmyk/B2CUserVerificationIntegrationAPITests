using System;
using System.Linq;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.DataAccess.Mappers
{
    internal interface IApplicationMapper
    {
        ApplicationDto MapToDto(Entities.Application entity);

        Application Map(Entities.Application entity);

        Entities.Application Map(Application application);

        void Update(Application application, Entities.Application entity);
    }

    internal class ApplicationMapper : IApplicationMapper
    {
        public ApplicationDto MapToDto(Entities.Application entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var tasks = entity.RequiredTasks.MapArray(ToDto);

            return new ApplicationDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                PolicyId = entity.PolicyId,
                ProductType = entity.ProductType,
                State = entity.State,
                PreviousState = entity.PreviousState,
                DecisionReasons = entity.DecisionReasons?.ToArray() ?? Array.Empty<string>(),
                IsAutomating = entity.IsAutomating,
                CreatedAt = entity.CreatedAt,
                Tasks = tasks
            };

            static ApplicationTaskDto ToDto(Entities.ApplicationTask task) => MapToDto(task.Task);
        }

        public Application Map(Entities.Application entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var requiredTasks = entity.RequiredTasks.MapArray(ToDomain);

            return new Application(
                entity.Id,
                entity.UserId,
                entity.PolicyId,
                entity.ProductType,
                entity.State,
                entity.PreviousState,
                requiredTasks,
                entity.DecisionReasons?.ToArray(),
                entity.IsAutomating);

            static ApplicationTask ToDomain(Entities.ApplicationTask task) => Map(task);
        }

        public Entities.Application Map(Application application)
        {
            if (application == null)
                throw new ArgumentNullException(nameof(application));

            var entity = new Entities.Application
            {
                Id = application.Id,
            };
            Update(application, entity);
            return entity;
        }

        public void Update(Application application, Entities.Application entity)
        {
            if (application == null)
                throw new ArgumentNullException(nameof(application));
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var requiredTasks = application.RequiredTasks.MapHashSet(ToEntity);

            entity.UserId = application.UserId;
            entity.PolicyId = application.PolicyId;
            entity.ProductType = application.ProductType;
            entity.State = application.State;
            entity.PreviousState = application.PreviousState;
            entity.DecisionReasons = application.DecisionReasons.ToHashSet();
            entity.RequiredTasks = requiredTasks;
            entity.IsAutomating = application.IsAutomating;
            
            Entities.ApplicationTask ToEntity(ApplicationTask task) => Map(task, application.Id);
        }

        private static Entities.ApplicationTask Map(ApplicationTask task, Guid applicationId)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            return new Entities.ApplicationTask
            {
                ApplicationId = applicationId,
                TaskId = task.Id
            };
        }

        private static ApplicationTask Map(Entities.ApplicationTask task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            return new ApplicationTask(task.TaskId, task.Task.State, task.Task.Type);
        }

        private static ApplicationTaskDto MapToDto(Entities.VerificationTask task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            return new ApplicationTaskDto
            {
                Id = task.Id,
                IsExpired = task.IsExpired,
                Result = task.Result,
                State = task.State,
                Type = task.Type,
                VariantId = task.VariantId
            };
        }
    }
}