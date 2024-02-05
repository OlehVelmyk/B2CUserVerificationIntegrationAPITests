using System;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Provider.Services.System
{
    internal sealed class DuplicateScreeningCheckRunner : SyncCheckRunner<DuplicateScreeningInputData>
    {
        private readonly IDuplicateSearchService _duplicateSearchService;

        public DuplicateScreeningCheckRunner(IDuplicateSearchService duplicateSearchService)
        {
            _duplicateSearchService = duplicateSearchService ?? throw new ArgumentNullException(nameof(duplicateSearchService));
        }

        protected override async Task<CheckProcessingResult> RunSync(DuplicateScreeningInputData checkData)
        {
            var context = DuplicateSearchContext.Create(checkData.UserId, checkData.FullName, checkData.BirthDate);
            var searchResult = await _duplicateSearchService.FindAsync(context);
            var outputData = new DuplicateScreeningOutputData { Matches = searchResult.Matches, Total = searchResult.Total };

            return !searchResult.Matches.Any()
                ? CheckProcessingResult.Passed(outputData)
                : CheckProcessingResult.Failed(outputData);
        }
    }
}