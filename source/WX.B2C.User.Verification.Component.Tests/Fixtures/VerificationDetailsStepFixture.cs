using System;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Builders;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Events.Internal.Events;

namespace WX.B2C.User.Verification.Component.Tests.Fixtures
{
    internal class VerificationDetailsStepFixture : CollectionStepFixture<VerificationDetailsCollectionStepVariantDto>
    {
        private readonly VerificationDetailsFixture _verificationDetailsFixture;
        private readonly EventsFixture _eventsFixture;

        public VerificationDetailsStepFixture(VerificationDetailsFixture verificationDetailsFixture, EventsFixture eventsFixture)
        {
            _verificationDetailsFixture = verificationDetailsFixture ?? throw new ArgumentNullException(nameof(verificationDetailsFixture));
            _eventsFixture = eventsFixture ?? throw new ArgumentNullException(nameof(eventsFixture));
        }

        public override Task CompleteAsync(Guid userId, VerificationDetailsCollectionStepVariantDto variant, Seed seed)
        {
            Action<Guid> assert = correlationId =>
            {
                variant.Dump();
                // Collection steps like IdDocNumber could be completed at moment of call by check.
                // Therefore correlation id is not always correct check 
                _eventsFixture.ShouldExistSingle<CollectionStepCompletedEvent>(e => e.CorrelationId == correlationId || 
                                                                                    (e.EventArgs.UserId == userId && e.EventArgs.XPath == variant.Property.GetXPath()) );
            };
            return SubmitAsync(userId, variant.Property, assert, seed);
        }

        public override Task MoveInReviewAsync(Guid userId, VerificationDetailsCollectionStepVariantDto variant, Seed seed)
        {
            Action<Guid> assert = correlationId =>
            {
                variant.Dump();
                _eventsFixture.ShouldExistSingle<CollectionStepReadyForReviewEvent>(correlationId);
            };
            return SubmitAsync(userId, variant.Property, assert, seed);
        }

        public async Task CompleteAsync(Guid userId, TaxResidence taxResidence)
        {
            await _verificationDetailsFixture.UpdateByUserAsync(userId, builder => builder.With(taxResidence));
            _eventsFixture.ShouldExistSingle(IsCollectionStepCompleted(userId, VerificationDetails.TaxResidence));
        }

        public async Task CompleteAsync(Guid userId, Tin tin)
        {
            await _verificationDetailsFixture.UpdateByUserAsync(userId, builder => builder.With(tin));
            _eventsFixture.ShouldExistSingle(IsCollectionStepCompleted(userId, VerificationDetails.Tin));
        }

        private static Func<CollectionStepCompletedEvent, bool> IsCollectionStepCompleted(Guid userId, string xpath) =>
            e => e.EventArgs.UserId == userId && e.EventArgs.XPath == xpath;

        private async Task SubmitAsync(Guid userId, VerificationDetailsProperty property, Action<Guid> assert, Seed seed)
        {
            var requestBuilder = property switch
            {
                VerificationDetailsProperty.TaxResidence => GetTaxResidenceBuilder(seed),
                VerificationDetailsProperty.Tin => GetTinBuilder(seed),
                VerificationDetailsProperty.IdDocumentNumber => GetIdDocumentNumberBuilder(seed),
                _ => null

            };

            if (requestBuilder is not null)
            {
                var correlationId = Guid.NewGuid();
                await _verificationDetailsFixture.UpdateByAdminAsync(userId, requestBuilder, correlationId);
                assert(correlationId);
            }
        }

        private static Func<AdminUpdateVerificationDetailsRequestBuilder, AdminUpdateVerificationDetailsRequestBuilder> GetTaxResidenceBuilder(Seed seed)
        {
            var faker = FakerFactory.Create(seed);
            var amount = faker.Random.Number(1, 4);
            var countries = CountryCodes.Supported.Where(cc => cc != CountryCodes.Us);
            var taxResidence = faker.PickRandom(countries, amount).ToArray();
            return builder => builder.With(taxResidence);
        }

        private static Func<AdminUpdateVerificationDetailsRequestBuilder, AdminUpdateVerificationDetailsRequestBuilder> GetTinBuilder(Seed seed)
        {
            var faker = FakerFactory.Create(seed);
            var tinType = Models.TinType.SSN; // To prevent "EDD questionnaire" trigger in US
            var tinNumber = TinFactory.Create(tinType, seed);
            var tin = new Tin
            {
                Type = tinType,
                Number = tinNumber
            };
            return builder => builder.With(tin);
        }

        private static Func<AdminUpdateVerificationDetailsRequestBuilder, AdminUpdateVerificationDetailsRequestBuilder> GetIdDocumentNumberBuilder(Seed seed)
        {
            var faker = FakerFactory.Create(seed);
            var documentType = faker.PickRandom(DocumentTypes.IdentityDocumentTypes);
            var documentNumber = faker.Random.Number(100000, 1000000000);
            var idDocNumber = new IdDocumentNumber
            {
                Type = documentType,
                Number = documentNumber.ToString()
            };
            return builder => builder.With(idDocNumber);
        }
    }
}
