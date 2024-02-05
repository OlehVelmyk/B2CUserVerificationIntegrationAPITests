using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.DataAccess.Mappers
{
    internal interface ITaskMapper
    {
        VerificationTask Map(Entities.VerificationTask entity);

        Entities.VerificationTask Map(VerificationTask task);

        void Update(VerificationTask source, Entities.VerificationTask target);

        TaskDto MapToDto(Entities.VerificationTask entity);
    }

    internal class TaskMapper : ITaskMapper
    {
        public VerificationTask Map(Entities.VerificationTask entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var collectionSteps = entity.CollectionSteps.MapArray(Map);
            var performedChecks = entity.PerformedChecks.MapArray(Map);
            var expirationDetails = Map(entity.ExpirationReason, entity.ExpiredAt);

            return new VerificationTask(
                entity.Id,
                entity.UserId,
                entity.VariantId,
                entity.Type,
                entity.CreationDate,
                entity.State,
                entity.Result,
                collectionSteps,
                performedChecks,
                expirationDetails);
        }

        public Entities.VerificationTask Map(VerificationTask task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            var entity = new Entities.VerificationTask { Id = task.Id };
            Update(task, entity);
            return entity;
        }

        public void Update(VerificationTask source, Entities.VerificationTask target)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.UserId = source.UserId;
            target.CreationDate = source.CreationDate;
            target.Type = source.Type;
            target.VariantId = source.VariantId;
            target.State = source.State;
            target.Result = source.Result;
            target.IsExpired = source.IsExpired;
            target.ExpirationReason = source.ExpirationDetails?.ExpirationReason;
            target.ExpiredAt = source.ExpirationDetails?.ExpiredAt;
            target.CollectionSteps = source.CollectionSteps.MapHashSet(MapTaskStep);

            Entities.TaskCollectionStep MapTaskStep(TaskCollectionStep step) => Map(step, source.Id);
        }

        public TaskDto MapToDto(Entities.VerificationTask entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var checks = entity.PerformedChecks?.MapArray(MapToDto);
            var collectionSteps = entity.CollectionSteps.MapArray(MapToDto);

            return new TaskDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                State = entity.State,
                Type = entity.Type,
                Result = entity.Result,
                VariantId = entity.VariantId,
                Checks = checks ?? Array.Empty<TaskCheckDto>(),
                CollectionSteps = collectionSteps,
                CreatedAt = entity.CreationDate
            };
        }

        private static TaskCheck Map(Entities.TaskCheck taskCheck)
        {
            if (taskCheck == null)
                throw new ArgumentNullException(nameof(taskCheck));

            return new TaskCheck(
                taskCheck.CheckId, 
                taskCheck.Check.VariantId, 
                taskCheck.Check.Type, 
                taskCheck.Check.State,
                taskCheck.Check.Result);
        }

        private static Entities.TaskCheck Map(TaskCheck taskCheck, Guid taskId)
        {
            if (taskCheck == null)
                throw new ArgumentNullException(nameof(taskCheck));

            return new Entities.TaskCheck { CheckId = taskCheck.Id, TaskId = taskId };
        }

        private static TaskCollectionStep Map(Entities.TaskCollectionStep step)
        {
            if (step == null)
                throw new ArgumentNullException(nameof(step));

            return new TaskCollectionStep(
                step.StepId, 
                step.Step.State, 
                step.Step.IsRequired);
        }

        private static TaskCheckDto MapToDto(Entities.TaskCheck entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new TaskCheckDto
            {
                Id = entity.CheckId,
                VariantId = entity.Check.VariantId,
                Type = entity.Check.Type,
                State = entity.Check.State,
                Provider = entity.Check.Provider,
                Result = entity.Check.Result,
                CreatedAt = entity.Check.CreatedAt,
                CompletedAt = entity.Check.CompletedAt
            };
        }

        private static TaskCollectionStepDto MapToDto(Entities.TaskCollectionStep entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var taskStep = entity.Step;
            return new TaskCollectionStepDto
            {
                Id = taskStep.Id,
                IsRequired = taskStep.IsRequired,
                State = taskStep.State,
                XPath = taskStep.XPath,
                IsReviewNeeded = taskStep.IsReviewNeeded,
                ReviewResult = taskStep.ReviewResult,
                RequestedAt = taskStep.CreatedAt,
                UpdatedAt = taskStep.UpdatedAt
            };
        }

        private static Entities.TaskCollectionStep Map(TaskCollectionStep step, Guid taskId)
        {
            if (step == null)
                throw new ArgumentNullException(nameof(step));

            return new Entities.TaskCollectionStep { StepId = step.Id, TaskId = taskId };
        }

        private static TaskExpirationDetails Map(TaskExpirationReason? expirationReason, DateTime? expiredAt)
        {
            if (!expirationReason.HasValue && !expiredAt.HasValue)
                return null;

            return new TaskExpirationDetails(expirationReason.GetValueOrDefault(), expiredAt.GetValueOrDefault());
        }
    }
}