using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Component.Tests.Helpers;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Component.Tests.Providers;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Events.Internal.Extensions;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Component.Tests.Fixtures
{
    internal class ChecksFixture
    {
        private readonly AdminApiClientFactory _adminApiClientFactory;
        private readonly AdministratorFactory _adminFactory;
        private readonly CollectionStepsFixture _stepsFixture;
        private readonly EventsFixture _eventsFixture;
        private readonly ICheckProvider _checkProvider;
        private readonly ICheckDataProvider _checkDataProvider;

        public ChecksFixture(AdminApiClientFactory adminApiClientFactory,
                             AdministratorFactory adminFactory,
                             CollectionStepsFixture stepsFixture,
                             EventsFixture eventsFixture,
                             ICheckProvider checkProvider,
                             ICheckDataProvider checkDataProvider)
        {
            _adminApiClientFactory = adminApiClientFactory ?? throw new ArgumentNullException(nameof(adminApiClientFactory));
            _adminFactory = adminFactory ?? throw new ArgumentNullException(nameof(adminFactory));
            _stepsFixture = stepsFixture ?? throw new ArgumentNullException(nameof(stepsFixture));
            _eventsFixture = eventsFixture ?? throw new ArgumentNullException(nameof(eventsFixture));
            _checkProvider = checkProvider ?? throw new ArgumentNullException(nameof(checkProvider));
            _checkDataProvider = checkDataProvider ?? throw new ArgumentNullException(nameof(checkDataProvider));
        }

        public async Task<Guid> RequestAsync(Guid userId, Guid variantId, Guid[] relatedTasks = null)
        {
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            var request = new CheckRequest
            {
                VariantId = variantId,
                RelatedTasks = relatedTasks ?? Array.Empty<Guid>(),
                Reason = $"{nameof(ChecksFixture)}.{nameof(RequestAsync)}"
            };

            await adminClient.Checks.RequestAsync(request, userId);

            return _eventsFixture.ShouldExistSingle<CheckCreatedEvent>(adminClient.CorrelationId).EventArgs.CheckId;
        }

        public async Task CompleteAsync(Guid userId, Guid checkId, Seed seed)
        {
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            var check = await adminClient.Checks.GetAsync(userId, checkId);
            if (check.State is CheckState.Complete or CheckState.Error)
                return;

            if (check.State is CheckState.Pending)
            {
                await CompleteAsync(userId, check.Variant.Id, seed, check.RelatedTasks.ToArray());
                return;
            }

            // Otherwise state is Running
            if (check.Variant.Provider is CheckProviderType.Onfido or CheckProviderType.PassFort)
                _eventsFixture.ShouldExist<CheckPerformedEvent>(e => e.EventArgs.CheckId == checkId); // TODO: WRXB-10696

            _eventsFixture.ShouldExist<CheckCompletedEvent>(e => e.EventArgs.CheckId == checkId); // TODO: WRXB-10696
        }

        public async Task<Guid> CompleteAsync(Guid userId, Guid variantId, Seed seed, Guid[] relatedTasks)
        {
            var checkId = await RunAsync(userId, variantId, seed, relatedTasks);

            var checkInfo = _checkProvider.Get(variantId);
            if (checkInfo.Provider is CheckProviderType.Onfido or CheckProviderType.PassFort)
                _eventsFixture.ShouldExist<CheckPerformedEvent>(e => e.EventArgs.CheckId == checkId); // TODO: WRXB-10696

            _eventsFixture.ShouldExist<CheckCompletedEvent>(e => e.EventArgs.CheckId == checkId); // TODO: WRXB-10696
            return checkId;
        }

        private async Task<Guid> RunAsync(Guid userId, Guid variantId, Seed seed, Guid[] relatedTasks = null)
        {
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            var checks = await adminClient.Checks.GetAllAsync(userId);
            var checkId = checks.Where(check => check.Variant.Id == variantId)
                                .Where(check => check.State is CheckState.Pending)
                                .Select(check => (Guid?)check.Id)
                                .FirstOrDefault();

            var steps = await _checkDataProvider.GetRequestedStepsAsync(userId, variantId);

            if (!checkId.HasValue)
                checkId = await RequestAsync(userId, variantId, relatedTasks);
            if (steps.Any())
                await CompleteStepsAsync(userId, steps, checks, seed);

            _eventsFixture.ShouldExist<CheckStartedEvent>(e => e.EventArgs.CheckId == checkId); // TODO: WRXB-10696
            return checkId.Value;
        }

        private async Task CompleteStepsAsync(Guid userId, (Guid id, CollectionStepVariantDto variant)[] steps, IList<CheckDto> checks, Seed seed)
        {
            var stepIds = steps.Select(step => step.id);
            if (!steps.Any(step => IsVerificationVariant(step.variant, VerificationDetailsProperty.Nationality)))
            {
                await _stepsFixture.CompleteAllAsync(userId, stepIds, true, seed);
                return;
            }

            var identityDocumentCheckVariantId = GetIdentityDocumentCheckVariantId();
            await CompleteAsync(userId, identityDocumentCheckVariantId, seed, null);
            // TODO: If rerequest VerificationDetails.Nationality step, second time it won`t be completed due to IdentityDocument check
            // TODO: Because we don`t generate new Nationality every time
            _eventsFixture.ShouldExistSingle<VerificationDetailsUpdatedEvent>(IsVerificationDetailsUpdated);
            _eventsFixture.ShouldExistSingle<CollectionStepCompletedEvent>(e => IsStepCompleted(e, VerificationDetails.Nationality));

            stepIds = steps
                .Where(step => !IsVerificationVariant(step.variant, VerificationDetailsProperty.Nationality))
                .Where(step => !IsVerificationVariant(step.variant, VerificationDetailsProperty.IdDocumentNumber))
                .Select(step => step.id);

            await _stepsFixture.CompleteAllAsync(userId, stepIds, true, seed);

            static bool IsVerificationVariant(CollectionStepVariantDto variant, VerificationDetailsProperty property) =>
                variant is VerificationDetailsCollectionStepVariantDto verificationDetailsVariant &&
                verificationDetailsVariant.Property == property;

            Guid GetIdentityDocumentCheckVariantId() =>
                checks.FirstOrDefault(c => c is { Type: CheckType.IdentityDocument, Variant: { Provider: CheckProviderType.Onfido } })?.Variant.Id ??
                _checkProvider.Get(CheckType.IdentityDocument, CheckProviderType.Onfido).VariantId;

            bool IsVerificationDetailsUpdated(VerificationDetailsUpdatedEvent e) =>
                e.EventArgs.UserId == userId &&
                e.EventArgs.Changes.Find<IdDocumentNumber>(VerificationDetails.IdDocumentNumber) is not null &&
                e.EventArgs.Changes.Find<string>(VerificationDetails.Nationality) is not null;

            bool IsStepCompleted(CollectionStepCompletedEvent e, string xPath) =>
                e.EventArgs.UserId == userId && e.EventArgs.XPath == xPath;
        }
    }
}
