using System;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Provider.Contracts;

namespace WX.B2C.User.Verification.Provider.Services.System
{
    internal sealed class IdNumberDuplicationCheckProviderFactory : BaseCheckProviderFactory<IdNumberDuplicationCheckConfiguration>
    {
        private readonly IDuplicateSearchService _duplicateSearchService;

        public IdNumberDuplicationCheckProviderFactory(IDuplicateSearchService duplicateSearchService)
        {
            _duplicateSearchService = duplicateSearchService ?? throw new ArgumentNullException(nameof(duplicateSearchService));
        }

        protected override CheckProvider Create(IdNumberDuplicationCheckConfiguration configuration)
        {
            var checkDataValidator = new IdNumberDuplicationCheckDataValidator(configuration);
            var checkRunner = new IdNumberDuplicationCheckRunner(_duplicateSearchService);
            return CheckProvider.Create(checkDataValidator, checkRunner);
        }
    }
}