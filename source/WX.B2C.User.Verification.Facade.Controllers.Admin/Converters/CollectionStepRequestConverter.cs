using System;
using System.Collections.Generic;
using System.Text.Json;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Enums;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Converters;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Converters
{
    public sealed class CollectionStepRequestConverter : PolymorphicDeserializer<CollectionStepRequest>
    {
        private static readonly Dictionary<string, Type> KnownTypes = new()
        {
            [nameof(CollectionStepType.VerificationDetails).ToLower()] = typeof(VerificationDetailsCollectionStepRequest),
            [nameof(CollectionStepType.PersonalDetails).ToLower()] = typeof(PersonalDetailsCollectionStepRequest),
            [nameof(CollectionStepType.Document).ToLower()] = typeof(DocumentCollectionStepRequest),
            [nameof(CollectionStepType.Survey).ToLower()] = typeof(SurveyCollectionStepRequest)
        };

        public CollectionStepRequestConverter()
            : base(nameof(CollectionStepRequest.Type).ToLower())
        {
        }

        protected override bool TryGetType(string discriminator, out Type resolvedType)
        {
            discriminator = discriminator?.ToLower() ?? string.Empty;

            if (!KnownTypes.TryGetValue(discriminator, out resolvedType))
                throw new JsonException("Cannot parse discriminator for CollectionStepRequest");

            return true;
        }
    }
}