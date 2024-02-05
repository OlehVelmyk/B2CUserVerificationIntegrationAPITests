using System;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Storages;

namespace WX.B2C.User.Verification.Provider.Services.Sandbox
{
    internal class DuplicateSearchServiceDecorator : IDuplicateSearchService
    {
        private readonly IDuplicateSearchService _inner;
        private readonly IOptionsProvider _optionsProvider;

        public DuplicateSearchServiceDecorator(IDuplicateSearchService inner, IOptionsProvider optionsProvider)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _optionsProvider = optionsProvider ?? throw new ArgumentNullException(nameof(optionsProvider));
        }

        public async Task<DuplicateSearchResult> FindAsync(DuplicateSearchContext context)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            var isExcluded = await IsExcluded(context.FullName);

            return isExcluded switch
            {
                false => await _inner.FindAsync(context),
                true when context.IdDocumentNumber is null => DuplicateSearchResult.Empty,
                true => await _inner.FindAsync(DuplicateSearchContext.Create(context.UserId, context.IdDocumentNumber))
            };
        }

        private async Task<bool> IsExcluded(FullNameDto fullName)
        {
            if (fullName is null)
                return false;

            var option = await _optionsProvider.GetAsync<ExcludedNamesOption>();
            return option.Names.Any(IsExcludedName);

            bool IsExcludedName(ExcludedNameOption excluded) =>
               (excluded.FirstName == ExcludedNameOption.AnyNameKey || fullName.FirstName?.ToUpper() == excluded.FirstName.ToUpper()) && 
               (excluded.LastName == ExcludedNameOption.AnyNameKey || fullName.LastName?.ToUpper() == excluded.LastName.ToUpper());
        }
    }
}
