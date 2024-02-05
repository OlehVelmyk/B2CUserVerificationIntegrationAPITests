using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.PassFort.Providers;
using WX.Core.TypeExtensions;

namespace WX.B2C.User.Verification.Listener.PassFort.Services
{
    internal interface IExternalProfileAdapter
    {
        public Task ExecuteAsync(Guid userId, Func<string, Task> action);

        public Task ExecuteAsync(Guid userId, Func<string, string, Task> action);
    }

    internal class ExternalProfileAdapter : IExternalProfileAdapter
    {
        private readonly IExternalProfileStorage _externalProfileStorage;
        private readonly IProfileStorage _profileStorage;
        private readonly IApplicationStorage _applicationStorage;
        private readonly IPassFortApplicationProductProvider _productProvider;
        private readonly ICountryDetailsProvider _countryDetailsProvider;

        public ExternalProfileAdapter(
            IExternalProfileStorage externalProfileStorage,
            IProfileStorage profileStorage,
            IApplicationStorage applicationStorage,
            IPassFortApplicationProductProvider productProvider,
            ICountryDetailsProvider countryDetailsProvider)
        {
            _externalProfileStorage = externalProfileStorage ?? throw new ArgumentNullException(nameof(externalProfileStorage));
            _profileStorage = profileStorage ?? throw new ArgumentNullException(nameof(profileStorage));
            _applicationStorage = applicationStorage ?? throw new ArgumentNullException(nameof(applicationStorage));
            _productProvider = productProvider ?? throw new ArgumentNullException(nameof(productProvider));
            _countryDetailsProvider = countryDetailsProvider ?? throw new ArgumentNullException(nameof(countryDetailsProvider));
        }

        public async Task ExecuteAsync(Guid userId, Func<string, Task> action)
        {
            if (!await ShouldBeSynced(userId))
                return;

            var externalProfile = await _externalProfileStorage.FindAsync(userId, ExternalProviderType.PassFort);
            if (externalProfile is not null)
            {
                var profileId = externalProfile.Id;
                await action(profileId);
            }
        }

        public Task ExecuteAsync(Guid userId, Func<string, string, Task> action) =>
            ExecuteAsync(userId, async profileId =>
            {
                var country = await _profileStorage.GetResidenceCountryAsync(userId);
                var region = await _countryDetailsProvider.GetRegionAsync(country);
                var productId = _productProvider.FindProductId(region, country);
                if (productId is null)
                    return;

                await action(profileId, productId.UnSecure());
            });

        /// <summary>
        /// TODO WRXB-10546 Remove in phase 2 when all users will be migrated
        /// </summary>
        private Task<bool> ShouldBeSynced(Guid userId) =>
            _applicationStorage.IsAutomatedAsync(userId, ProductType.WirexBasic);
    }
}