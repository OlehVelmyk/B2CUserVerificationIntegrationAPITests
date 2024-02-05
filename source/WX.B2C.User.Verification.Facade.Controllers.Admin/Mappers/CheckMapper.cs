using System;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos;
using CoreDtos = WX.B2C.User.Verification.Core.Contracts.Dtos;
using PolicyDtos = WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers
{
    public interface ICheckMapper
    {
        Task<CheckDto> Map(CoreDtos.CheckDto check, PolicyDtos.VariantNameDto checkVariant, Guid[] relatedTasks);
    }

    internal class CheckMapper : ICheckMapper
    {
        private readonly ICollectionStepBriefDataMapper _collectionStepBriefDataMapper;
        private readonly IDocumentMapper _documentMapper;
        private readonly IXPathParser _xPathParser;

        public CheckMapper(
            ICollectionStepBriefDataMapper collectionStepBriefDataMapper,
            IDocumentMapper documentMapper,
            IXPathParser xPathParser)
        {
            _collectionStepBriefDataMapper = collectionStepBriefDataMapper ?? throw new ArgumentNullException(nameof(collectionStepBriefDataMapper));
            _documentMapper = documentMapper ?? throw new ArgumentNullException(nameof(documentMapper));
            _xPathParser = xPathParser ?? throw new ArgumentNullException(nameof(xPathParser));
        }

        public async Task<CheckDto> Map(CoreDtos.CheckDto check, PolicyDtos.VariantNameDto checkVariant, Guid[] relatedTasks)
        {
            if (check == null)
                throw new ArgumentNullException(nameof(check));
            if (checkVariant == null)
                throw new ArgumentNullException(nameof(checkVariant));

            var (inputData, inputDocuments) = await MapInputData(check.InputData);

            var errors = check.Errors?.Select(error => new CheckErrorDto
            {
                Code = error.Code,
                Message = error.Message
            }).ToArray();

            return new CheckDto
            {
                Id = check.Id,
                Type = check.Type,
                Variant = new CheckVariantDto
                {
                    Id = check.Variant.Id,
                    Name = checkVariant.Name,
                    Provider = check.Variant.Provider
                },
                State = check.State,
                Result = check.Result,
                Decision = check.Decision,
                InputData = inputData,
                InputDocuments = inputDocuments,
                OutputData = check.OutputData,
                Errors = errors,
                RelatedTasks = relatedTasks,
                CreatedAt = check.CreatedAt,
                StartedAt = check.StartedAt,
                PerformedAt = check.PerformedAt,
                CompletedAt = check.CompletedAt
            };
        }

        private async Task<(CollectionStepBriefDataDto[], DocumentDto[])> MapInputData(CoreDtos.CheckInputDataDto inputData)
        {
            if (inputData == null)
                throw new ArgumentNullException(nameof(inputData));

            var inputValues = inputData
                               .Where(pair => _xPathParser.Parse(pair.Key) is not DocumentsXPathDetails)
                               .ToDictionary(pair => pair.Key, pair => pair.Value);

            var inputDocuments = inputData
                            .Where(pair => _xPathParser.Parse(pair.Key) is DocumentsXPathDetails)
                            .Select(pair => pair.Value)
                            .Cast<CoreDtos.DocumentDto>()
                            .ToArray();

            var mapped = await _collectionStepBriefDataMapper.MapAsync(inputValues);
            var mappedValues = mapped.Select(pair => pair.Value).ToArray();
            var mappedDocuments = inputDocuments.Select(_documentMapper.Map).ToArray();

            return (mappedValues, mappedDocuments);
        }
    }
}
