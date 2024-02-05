using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Public.Client;
using WX.B2C.User.Verification.Component.Tests.Builders;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Events.Internal.Events;

namespace WX.B2C.User.Verification.Component.Tests.Fixtures
{
    internal class VerificationDetailsFixture
    {
        private readonly AdminApiClientFactory _adminApiClientFactory;
        private readonly AdministratorFactory _adminFactory;
        private readonly PublicApiClientFactory _publicApiClientFactory;
        private readonly EventsFixture _eventsFixture;

        public VerificationDetailsFixture(AdminApiClientFactory adminApiClientFactory,
                                               AdministratorFactory adminFactory,
                                               PublicApiClientFactory publicApiClientFactory,
                                               EventsFixture eventsFixture)
        {
            _adminApiClientFactory = adminApiClientFactory ?? throw new ArgumentNullException(nameof(adminApiClientFactory));
            _adminFactory = adminFactory ?? throw new ArgumentNullException(nameof(adminFactory));
            _publicApiClientFactory = publicApiClientFactory ?? throw new ArgumentNullException(nameof(publicApiClientFactory));
            _eventsFixture = eventsFixture ?? throw new ArgumentNullException(nameof(eventsFixture));
        }

        public async Task UpdateByAdminAsync(Guid userId,
                                             Func<AdminUpdateVerificationDetailsRequestBuilder, AdminUpdateVerificationDetailsRequestBuilder> buildRequest,
                                             Guid? correlationId = null)
        {
            correlationId ??= Guid.NewGuid();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin, correlationId);
            var requestBuilder = new AdminUpdateVerificationDetailsRequestBuilder();

            var request = buildRequest(requestBuilder).Build();
            await client.Profile.UpdateAsync(request, userId);
            _eventsFixture.ShouldExistSingle<VerificationDetailsUpdatedEvent>(correlationId.Value);
        }

        public async Task UpdateByUserAsync(Guid userId,
                                            Func<UserUpdateVerificationDetailsRequestBuilder, UserUpdateVerificationDetailsRequestBuilder> buildRequest)
        {
            var correlationId = Guid.NewGuid();
            var client = _publicApiClientFactory.Create(userId, correlationId: correlationId);
            var requestBuilder = new UserUpdateVerificationDetailsRequestBuilder();

            var request = buildRequest(requestBuilder).Build();
            await client.Profile.UpdateAsync(request);
            _eventsFixture.ShouldExistSingle<VerificationDetailsUpdatedEvent>(correlationId);
        }
    }
}