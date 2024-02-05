using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.Provider.Contracts.IoC;

namespace WX.B2C.User.Verification.Automation.Services
{
    internal interface ICheckOutputExtractionService
    {
        bool TryExtract(CheckDto checkDto, out IReadOnlyDictionary<string, object> data);
    }

    internal class CheckOutputExtractionService : ICheckOutputExtractionService
    {
        private readonly IDictionary<CheckProviderMetadata, ICheckOutputDataExtractor> _dataExtractors;

        public CheckOutputExtractionService(IEnumerable<Lazy<ICheckOutputDataExtractor, CheckProviderMetadata>> dataExtractors)
        {
            var extractors = dataExtractors?.ToDictionary(x => x.Metadata, x => x.Value);
            _dataExtractors = extractors ?? throw new ArgumentNullException(nameof(dataExtractors));
        }

        public bool TryExtract(CheckDto checkDto, out IReadOnlyDictionary<string, object> data)
        {
            if (checkDto == null)
                throw new ArgumentNullException(nameof(checkDto));

            data = null;
            var metadata = new CheckProviderMetadata(checkDto.Type, checkDto.Variant.Provider);
            if (!_dataExtractors.TryGetValue(metadata, out var dataExtractor))
                return false;

            data = dataExtractor.Extract(checkDto.OutputData);
            return true;
        }
    }
}
