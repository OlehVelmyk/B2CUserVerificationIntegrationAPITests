using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Profile;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Events.Internal.Dtos;
using WX.B2C.User.Verification.Events.Internal.Extensions;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.PassFort;
using static WX.B2C.User.Verification.PassFort.Constants.Tags;

namespace WX.B2C.User.Verification.Listener.PassFort.Services
{
    public interface IPassFortTagService
    {
        Task CreateNewProfileTagsAsync(Guid userId, string profileId);

        Task SetOnfidoTagAsync(Guid userId, string profileId);

        Task UpdatePersonalDetailsTagsAsync(string profileId, PropertyChangeDto[] changes);

        Task UpdateVerificationDetailsTagsAsync(string profileId, PropertyChangeDto[] changes);

        Task AddAccountClosureTagAsync(string profileId);
    }

    internal class PassFortTagService : IPassFortTagService
    {
        //TODO move to configuration
        private static readonly Dictionary<string, string> RegionsTags = new()
        {
            { "APAC", ApacResidenceTag },
            { "US", USResidenceTag }
        };

        private readonly IPassFortTagGateway _gateway;
        private readonly IProfileStorage _profileStorage;
        private readonly IExternalProfileProvider _externalProfileProvider;
        private readonly IExternalProfileStorage _externalProfileStorage;
        private readonly ICountryDetailsProvider _countryDetailsProvider;

        public PassFortTagService(IPassFortTagGateway passFortTagsGateway,
                                  IProfileStorage profileStorage,
                                  IExternalProfileProvider externalProfileProvider,
                                  IExternalProfileStorage externalProfileStorage,
                                  ICountryDetailsProvider countryDetailsProvider)
        {
            _gateway = passFortTagsGateway ?? throw new ArgumentNullException(nameof(passFortTagsGateway));
            _profileStorage = profileStorage ?? throw new ArgumentNullException(nameof(profileStorage));
            _externalProfileProvider = externalProfileProvider ?? throw new ArgumentNullException(nameof(externalProfileProvider));
            _externalProfileStorage = externalProfileStorage ?? throw new ArgumentNullException(nameof(externalProfileStorage));
            _countryDetailsProvider = countryDetailsProvider ?? throw new ArgumentNullException(nameof(countryDetailsProvider));
        }

        public async Task CreateNewProfileTagsAsync(Guid userId, string profileId)
        {
            if (profileId == null)
                throw new ArgumentNullException(nameof(profileId));

            await SetOnfidoTagAsync(userId, profileId);

            var personalDetails = await _profileStorage.GetPersonalDetailsAsync(userId);
            await _gateway.AddTagAsync(profileId, personalDetails.Email);

            var userRegion = await _countryDetailsProvider.GetRegionAsync(personalDetails.ResidenceAddress.Country);
            await UpdateUserRegionTagsAsync(profileId, userRegion);

            var verificationDetails = await _profileStorage.GetVerificationDetailsAsync(userId);
            if (verificationDetails.RiskLevel is { } riskLevel)
                await UpdateRiskLevelTag(profileId, riskLevel);
        }

        public async Task SetOnfidoTagAsync(Guid userId, string profileId)
        {
            var onfidoId = await _externalProfileStorage.FindExternalIdAsync(userId, ExternalProviderType.Onfido);
            if (onfidoId != null)
                await _gateway.AddTagAsync(profileId, GetOnfidoTag(onfidoId));
        }

        public async Task UpdatePersonalDetailsTagsAsync(string profileId, PropertyChangeDto[] changes)
        {
            if (changes.Find<string>(XPathes.Email) is { } emailChange)
            {
                if (emailChange.PreviousValue is { } previousValue)
                    await _gateway.RemoveTagByPrefix(profileId, previousValue);
                await _gateway.AddTagAsync(profileId, emailChange.NewValue);
            }

            if (changes.Find<AddressDto>(XPathes.ResidenceAddress) is { } addressChange)
            {
                var newAddress = addressChange.NewValue;
                var userResidenceCounty = newAddress.Country;
                var userRegion = await _countryDetailsProvider.GetRegionAsync(userResidenceCounty);

                await UpdateUserRegionTagsAsync(profileId, userRegion);
            }
        }

        public Task UpdateVerificationDetailsTagsAsync(string profileId, PropertyChangeDto[] changes)
        {
            if (changes.Find<RiskLevel?>(XPathes.RiskLevel) is not { } riskLevelChange) 
                return Task.CompletedTask;

            var riskLevel = riskLevelChange.NewValue;
            return UpdateRiskLevelTag(profileId, riskLevel);
        }

        public Task AddAccountClosureTagAsync(string profileId) => _gateway.AddTagAsync(profileId, AccountClosureTag);

        private Task UpdateRiskLevelTag(string profileId, RiskLevel? riskLevel)
        {
            if (!riskLevel.HasValue) 
                return _gateway.RemoveTagByName(profileId, RiskLevelTags.AllTags);

            var riskLevelTag = RiskLevelTags.GetTag(riskLevel.Value);
            return _gateway.AddOrUpdateTagAsync(profileId, riskLevelTag, RiskLevelTags.AllTags);
        }

        private async Task UpdateUserRegionTagsAsync(string profileId, string userRegion)
        {
            if (RegionsTags.TryGetValue(userRegion, out var tag))
                await _gateway.AddOrUpdateTagAsync(profileId, tag, new []{ tag });

            await RegionsTags.Values
                             .Where(regionTag => regionTag != tag)
                             .Foreach(regionTag => _gateway.RemoveTagByPrefix(profileId, regionTag));
        }
    }
}