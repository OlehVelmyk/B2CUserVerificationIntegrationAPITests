using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Providers;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Services.Providers
{
    public class PolicySelectionContextProvider : IPolicySelectionContextProvider
    {
        private readonly IProfileStorage _profileStorage;
        private readonly ICountryDetailsProvider _countryDetailsProvider;

        public PolicySelectionContextProvider(IProfileStorage profileStorage, ICountryDetailsProvider countryDetailsProvider)
        {
            _profileStorage = profileStorage ?? throw new ArgumentNullException(nameof(profileStorage));
            _countryDetailsProvider = countryDetailsProvider ?? throw new ArgumentNullException(nameof(countryDetailsProvider));
        }

        public async Task<VerificationPolicySelectionContext> GetVerificationContextAsync(Guid userId)
        {
            var (address, region) = await FindResidenceAddressAsync(userId);
            return VerificationPolicySelectionContext.Create(address?.Country, region, address?.State);
        }

        public async Task<ValidationPolicySelectionContext> GetValidationContextAsync(Guid userId)
        {
            var (address, region) = await FindResidenceAddressAsync(userId);
            return ValidationPolicySelectionContext.Create(address?.Country, region);
        }

        public async Task<MonitoringPolicySelectionContext> GetMonitoringContextAsync(Guid userId)
        {
            var (address, region) = await FindResidenceAddressAsync(userId);
            return MonitoringPolicySelectionContext.Create(address?.Country, region);
        }

        private async Task<(AddressDto, string)> FindResidenceAddressAsync(Guid userId)
        {
            var residenceAddress = await _profileStorage.FindResidenceAddressAsync(userId);
            if (residenceAddress?.Country is null)
                return (residenceAddress, null);

            var region = await _countryDetailsProvider.GetRegionAsync(residenceAddress.Country);
            return (residenceAddress, region);
        }
    }
}