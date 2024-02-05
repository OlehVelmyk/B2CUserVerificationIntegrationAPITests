using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Checks;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Automation.Services.Validators
{
    internal static class ApplicationRejectionReasons
    {
        public const string Fraud = nameof(Fraud);
        public const string UserLocked = nameof(UserLocked);
        public const string InstantIdClosing = nameof(InstantIdClosing);
        public const string DuplicateAccount = "Duplicate account";
        public const string VerificationPolicyChanged = nameof(VerificationPolicyChanged);
    }

    public interface IApplicationRejectionValidator
    {
        Task<string> ValidatePoiIssuingCountryAsync(string poiIssuingCountry);

        Task<string> ValidateCheckResultAsync(Guid checkId);
    }

    internal class ApplicationRejectionValidator : IApplicationRejectionValidator
    {
        private readonly IOptionsProvider _optionsProvider;
        private readonly ICheckStorage _checkStorage;

        public ApplicationRejectionValidator(IOptionsProvider optionsProvider, 
                                             ICheckStorage checkStorage)
        {
            _optionsProvider = optionsProvider ?? throw new ArgumentNullException(nameof(optionsProvider));
            _checkStorage = checkStorage ?? throw new ArgumentNullException(nameof(checkStorage));
        }

        public async Task<string> ValidatePoiIssuingCountryAsync(string poiIssuingCountry)
        {
            if (poiIssuingCountry == null)
                throw new ArgumentNullException(nameof(poiIssuingCountry));

            var blacklistOption = await _optionsProvider.GetAsync<BlacklistedCountriesOption>();

            return poiIssuingCountry.In(blacklistOption.Countries) 
                ? ApplicationRejectionReasons.Fraud 
                : null;
        }

        public async Task<string> ValidateCheckResultAsync(Guid checkId)
        {
            var check = await _checkStorage.GetAsync(checkId);

            return check.Decision switch
            {
                CheckDecisions.Fraud            => ApplicationRejectionReasons.Fraud,
                CheckDecisions.DuplicateAccount => ApplicationRejectionReasons.DuplicateAccount,
                CheckDecisions.InstantIdClosing => ApplicationRejectionReasons.InstantIdClosing,
                _                               => null
            };
        }
    }
}
