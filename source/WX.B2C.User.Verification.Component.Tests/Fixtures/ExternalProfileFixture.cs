using System;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Api.Public.Client;
using WX.B2C.User.Verification.Api.Public.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Models;
using AdminApi = WX.B2C.User.Verification.Api.Admin.Client.Models;

namespace WX.B2C.User.Verification.Component.Tests.Fixtures
{
    internal class ExternalProfileFixture
    {
        private readonly PublicApiClientFactory _publicApiClientFactory;
        private readonly AdminApiClientFactory _adminApiClientFactory;
        private readonly AdministratorFactory _adminFactory;
        private readonly ChecksFixture _checksFixture;

        public ExternalProfileFixture(PublicApiClientFactory publicApiClientFactory,
                                      AdminApiClientFactory adminApiClientFactory,
                                      AdministratorFactory adminFactory,
                                      ChecksFixture checksFixture)
        {
            _publicApiClientFactory = publicApiClientFactory ?? throw new ArgumentNullException(nameof(publicApiClientFactory));
            _adminApiClientFactory = adminApiClientFactory ?? throw new ArgumentNullException(nameof(adminApiClientFactory));
            _adminFactory = adminFactory ?? throw new ArgumentNullException(nameof(adminFactory));
            _checksFixture = checksFixture ?? throw new ArgumentNullException(nameof(checksFixture));
        }

        public async Task<string> GetApplicantIdAsync(Guid userId)
        {
            var client = _publicApiClientFactory.Create(userId);

            var request = new SdkTokenRequest(TokenType.Web);
            var sdkToken = await client.Providers.PostAsync(request);

            return sdkToken.ApplicantId;
        }

        public async Task<string> GetProfileIdAsync(Guid userId, bool hardCreate = false, Seed seed = null)
        {
            if(hardCreate && seed is null)
                throw new ArgumentNullException(nameof(seed));

            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            if (hardCreate)
            {
                var checks = await adminClient.ExecuteUntilAsync(
                    client => client.Checks.GetAllAsync(userId),
                    checks => checks.Any(IsRiskListsScreeningCheck));
                var check = checks.First(IsRiskListsScreeningCheck);
                await _checksFixture.CompleteAsync(userId, check.Variant.Id, seed, null);
            }

            var profiles = await adminClient.ExternalProfiles.GetAsync(userId);
            var profile = profiles.FirstOrDefault(profile => profile.Name == AdminApi.ExternalProviderType.PassFort);
            if (profile is null)
                throw new InvalidOperationException("Cannot find PassFort profile");

            var linkParts = profile.Link.Split("/");
            if(linkParts.Length == 0)
                throw new InvalidOperationException("Cannot find PassFort profile id");

            return linkParts[^2];

            bool IsRiskListsScreeningCheck(CheckDto check) =>
                check.Type == CheckType.RiskListsScreening && check.Variant.Provider == CheckProviderType.PassFort;
        }
    }
}
