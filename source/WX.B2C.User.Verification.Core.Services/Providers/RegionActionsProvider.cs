using System;
using System.Linq;
using System.Threading.Tasks;
using Optional;
using Optional.Unsafe;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Core.Contracts.Providers;
using WX.B2C.User.Verification.Core.Contracts.Storages;

namespace WX.B2C.User.Verification.Core.Services.Providers
{
    public class RegionActionsProvider : IRegionActionsProvider
    {
        private readonly IProfileStorage _profileStorage;
        private readonly ICountryDetailsProvider _countryDetailsProvider;
        private readonly IOptionsProvider _optionsProvider;

        public RegionActionsProvider(IProfileStorage profileStorage,
                                     ICountryDetailsProvider countryDetailsProvider,
                                     IOptionsProvider optionsProvider)
        {
            _profileStorage = profileStorage ?? throw new ArgumentNullException(nameof(profileStorage));
            _countryDetailsProvider = countryDetailsProvider ?? throw new ArgumentNullException(nameof(countryDetailsProvider));
            _optionsProvider = optionsProvider ?? throw new ArgumentNullException(nameof(optionsProvider));
        }

        public async Task<RegionActionsOption> GetAsync(Guid userId)
        {
            var residenceAddress = await _profileStorage.GetResidenceAddressAsync(userId);
            var region = await _countryDetailsProvider.GetRegionAsync(residenceAddress.Country);

            var actionsOption = await _optionsProvider.GetAsync<ActionsOption>();
            var regionActions = residenceAddress.State.SomeNotNull()
                                                .FlatMap(FindByState)
                                                .Else(FindByCountry)
                                                .Else(FindByRegion)
                                                .Else(FindGlobalPredicate);

            return regionActions.ValueOrFailure($"Cannot find actions option for user {userId}. "
                                              + $"State:{residenceAddress.State}, "
                                              + $"Country{residenceAddress.Country}, "
                                              + $"Region{region}");

            Option<RegionActionsOption> FindByState(string state) =>
                Find(option => option.RegionType == RegionType.State && option.Region == state);

            Option<RegionActionsOption> FindByCountry() =>
                Find(option => option.RegionType == RegionType.Country && option.Region == residenceAddress.Country);

            Option<RegionActionsOption> FindByRegion() =>
                Find(option => option.RegionType == RegionType.Region && option.Region == region);

            Option<RegionActionsOption> FindGlobalPredicate() =>
                Find(option => option.RegionType == RegionType.Global);

            Option<RegionActionsOption> Find(Func<RegionActionsOption, bool> predicate) =>
                actionsOption.RegionActions.FirstOrDefault(predicate).SomeNotNull();
        }
    }
}
