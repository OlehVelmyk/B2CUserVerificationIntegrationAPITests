using System;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Provider.Contracts;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Provider.Services.System
{
    internal sealed class IdNumberDuplicationCheckRunner : SyncCheckRunner<IdNumberDuplicationInputData>
    {
        private readonly IDuplicateSearchService _duplicateSearchService;

        public IdNumberDuplicationCheckRunner(IDuplicateSearchService duplicateSearchService)
        {
            _duplicateSearchService = duplicateSearchService ?? throw new ArgumentNullException(nameof(duplicateSearchService));
        }

        protected override async Task<CheckProcessingResult> RunSync(IdNumberDuplicationInputData checkData)
        {
            if (checkData.IdDocumentNumber == IdDocumentNumberDto.NotPresented)
                return CheckProcessingResult.Failed(IdNumberDuplicationOutputData.NotPresented());

            var context = DuplicateSearchContext.Create(checkData.UserId, checkData.IdDocumentNumber);
            var searchResult = await _duplicateSearchService.FindAsync(context);
            var outputData = searchResult.Matches.Any()
                ? IdNumberDuplicationOutputData.MatchFound(searchResult.Matches, searchResult.Total)
                : new IdNumberDuplicationOutputData();

            return !searchResult.Matches.Any()
                ? CheckProcessingResult.Passed(outputData)
                : CheckProcessingResult.Failed(outputData);
        }
    }
}