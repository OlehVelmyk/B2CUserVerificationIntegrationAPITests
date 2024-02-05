using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using OutputCheckVariantDto = WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos.CheckVariantDto;
using OutputTaskCheckDto = WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos.TaskCheckDto;
using OutputTaskCollectionStepDto = WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos.TaskCollectionStepDto;
using OutputTaskDto = WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos.TaskDto;
using OutputTaskVariantDto = WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos.TaskVariantDto;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers
{
    public interface ITaskMapper
    {
        OutputTaskDto Map(
            TaskDto task,
            TaskVariantDto taskVariant,
            IEnumerable<VariantNameDto> checkVariantNames,
            IReadOnlyDictionary<string, string> collectionStepFormattedData);
    }

    internal class TaskMapper : ITaskMapper
    {
        private readonly ICollectionStepVariantMapper _stepVariantMapper;
        private readonly IXPathParser _xPathParser;

        public TaskMapper(ICollectionStepVariantMapper stepVariantMapper,
                          IXPathParser xPathParser)
        {
            _stepVariantMapper = stepVariantMapper ?? throw new ArgumentNullException(nameof(stepVariantMapper));
            _xPathParser = xPathParser ?? throw new ArgumentNullException(nameof(xPathParser));
        }

        public OutputTaskDto Map(
            TaskDto task,
            TaskVariantDto taskVariant,
            IEnumerable<VariantNameDto> checkVariantNames,
            IReadOnlyDictionary<string, string> collectionStepFormattedData)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));
            if (taskVariant == null)
                throw new ArgumentNullException(nameof(taskVariant));
            if (checkVariantNames == null)
                throw new ArgumentNullException(nameof(checkVariantNames));

            var collectionSteps = task.CollectionSteps
                                      .Select(collectionStep => Map(collectionStep, collectionStepFormattedData))
                                      .OrderByDescending(x => x.UpdatedAt)
                                      .ToArray();

            var checks = task.Checks
                             .Select(check => Map(check, checkVariantNames))
                             .OrderByDescending(x => x.CreatedAt)
                             .ToArray();

            return new OutputTaskDto
            {
                Id = task.Id,
                Type = task.Type,
                Variant = new OutputTaskVariantDto
                {
                    Id = taskVariant.VariantId,
                    Name = taskVariant.TaskName
                },
                Priority = taskVariant.Priority,
                State = task.State,
                CollectionSteps = collectionSteps,
                Checks = checks,
                Result = task.Result,
                CreatedAt = task.CreatedAt
            };
        }

        private OutputTaskCollectionStepDto Map(TaskCollectionStepDto taskStep,
                                                IReadOnlyDictionary<string, string> formattedData)
        {
            if (taskStep == null)
                throw new ArgumentNullException(nameof(taskStep));

            var details = _xPathParser.Parse(taskStep.XPath);
            // TODO: Fix serialization, now it serialize as base class
            var variant = _stepVariantMapper.Map(details);

            return new OutputTaskCollectionStepDto
            {
                Id = taskStep.Id,
                State = taskStep.State,
                ReviewResult = taskStep.ReviewResult,
                IsRequired = taskStep.IsRequired,
                IsReviewNeeded = taskStep.IsReviewNeeded,
                RequestedAt = taskStep.RequestedAt,
                UpdatedAt = taskStep.UpdatedAt,
                Variant = variant,
                // TODO: Fill data
                Data = default,
                FormattedData = formattedData.GetValueOrDefault(details.XPath)
            };
        }

        private static OutputTaskCheckDto Map(TaskCheckDto check, IEnumerable<VariantNameDto> checkVariantNames)
        {
            if (check == null)
                throw new ArgumentNullException(nameof(check));
            if (checkVariantNames == null)
                throw new ArgumentNullException(nameof(checkVariantNames));

            var checkVariantName = checkVariantNames.FirstOrDefault(name => name.Id == check.VariantId);

            return new OutputTaskCheckDto
            {
                Id = check.Id,
                Type = check.Type,
                Variant = new OutputCheckVariantDto
                {
                    Id = check.VariantId,
                    Name = checkVariantName?.Name,
                    Provider = check.Provider
                },
                State = check.State,
                Result = check.Result,
                CreatedAt = check.CreatedAt,
                CompletedAt = check.CompletedAt
            };
        }
    }
}
