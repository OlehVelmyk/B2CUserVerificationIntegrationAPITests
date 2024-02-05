using System;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Provider.Contracts;

namespace WX.B2C.User.Verification.Provider.Services.System
{
    internal sealed class DuplicateScreeningCheckProviderFactory : BaseCheckProviderFactory<DuplicateScreeningCheckConfiguration>
    {
        private readonly IDuplicateSearchService _duplicateSearchService;

        public DuplicateScreeningCheckProviderFactory(
            IDuplicateSearchService duplicateSearchService)
        {
            _duplicateSearchService = duplicateSearchService ?? throw new ArgumentNullException(nameof(duplicateSearchService));
        }

        protected override CheckProvider Create(DuplicateScreeningCheckConfiguration configuration)
        {
            var checkDataValidator = new DuplicateScreeningCheckDataValidator(configuration);
            var checkRunner = new DuplicateScreeningCheckRunner(_duplicateSearchService);
            return CheckProvider.Create(checkDataValidator, checkRunner);
        }
    }
}