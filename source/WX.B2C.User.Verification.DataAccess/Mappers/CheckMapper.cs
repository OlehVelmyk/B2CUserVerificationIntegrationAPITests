using System;
using System.Linq;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Domain.Checks;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.DataAccess.Mappers
{
    internal interface ICheckMapper
    {
        Check Map(Entities.Check entity);

        Entities.Check Map(Check check);

        Entities.TaskCheck[] MapToTasksChecks(Check check);

        void Update(Check source, Entities.Check target);

        CheckDto MapToDto(Entities.Check entity);
    }

    internal class CheckMapper : ICheckMapper
    {
        public Check Map(Entities.Check entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var variant = CheckVariant.Create(entity.VariantId, entity.Provider);
            var checkErrors = entity.Errors?.Select(MapError).ToArray();

            var executionContext = entity.InputData != null
                ? CheckExecutionContext.Create(entity.InputData, entity.ExternalData)
                : null;
            var processingResult = entity.Result.HasValue
                ? CheckProcessingResult.Create(entity.Result.Value, entity.Decision, entity.OutputData)
                : null;

            return new Check(entity.Id,
                entity.UserId,
                entity.Type,
                variant,
                entity.State,
                executionContext,
                processingResult,
                checkErrors,
                entity.StartedAt,
                entity.PerformedAt,
                entity.CompletedAt);

            static CheckError MapError(Entities.CheckError error) => CheckError.Create(error.Code, error.Message);
        }

        public Entities.Check Map(Check check)
        {
            if (check == null)
                throw new ArgumentNullException(nameof(check));

            var entity = new Entities.Check { Id = check.Id };
            Update(check, entity);
            return entity;
        }

        public Entities.TaskCheck[] MapToTasksChecks(Check check)
        {
            if (check == null)
                throw new ArgumentNullException(nameof(check));

            return check.RelatedTasks?.MapArray(ToEntity) ?? Array.Empty<Entities.TaskCheck>();

            Entities.TaskCheck ToEntity(Guid taskId) => new() { CheckId = check.Id, TaskId = taskId };
        }

        public void Update(Check source, Entities.Check target)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.UserId = source.UserId;
            target.Type = source.Type;
            target.VariantId = source.Variant.Id;
            target.State = source.State;
            target.Provider = source.Variant.Provider;
            target.ExternalId = source.ExecutionContext?.ExternalData?.GetStringValue(ExternalCheckProperties.ExternalId);
            target.InputData = source.ExecutionContext?.InputData?.ToDictionary();
            target.ExternalData = source.ExecutionContext?.ExternalData?.ToDictionary();
            target.Errors = source.Errors?.Select(MapError).ToArray();
            target.Result = source.ProcessingResult?.Result;
            target.Decision = source.ProcessingResult?.Decision;
            target.OutputData = source.ProcessingResult?.OutputData;
            target.StartedAt = source.StartedAt;
            target.PerformedAt = source.PerformedAt;
            target.CompletedAt = source.CompletedAt;

            static Entities.CheckError MapError(CheckError error) => new()
            {
                Code = error.Code,
                Message = error.Message,
                AdditionalData = error.AdditionalData
            };
        }

        public CheckDto MapToDto(Entities.Check entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var checkVariant = new CheckVariantDto
            {
                Id = entity.VariantId,
                Provider = entity.Provider
            };

            var inputData = entity.InputData != null 
                ? new CheckInputDataDto(entity.InputData) 
                : new CheckInputDataDto();

            var externalData = entity.ExternalData != null
                ? new CheckExternalDataDto(entity.ExternalData)
                : new CheckExternalDataDto();

            var errors = entity.Errors?.Select(MapError).ToArray();

            return new CheckDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Type = entity.Type,
                State = entity.State,
                Result = entity.Result,
                Decision = entity.Decision,
                Variant = checkVariant,
                ExternalData = externalData,
                InputData = inputData,
                OutputData = entity.OutputData,
                Errors = errors,
                CreatedAt = entity.CreatedAt,
                StartedAt = entity.StartedAt,
                PerformedAt = entity.PerformedAt,
                CompletedAt = entity.CompletedAt
            };

            static CheckErrorDto MapError(Entities.CheckError error) => new()
            {
                Code = error.Code,
                Message = error.Message,
                AdditionalData = error.AdditionalData != null
                    ? new ErrorAdditionalDataDto(error.AdditionalData.ToDictionary())
                    : null
            };
        }
    }
}