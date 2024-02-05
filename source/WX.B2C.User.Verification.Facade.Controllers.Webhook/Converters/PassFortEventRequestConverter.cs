using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Facade.Controllers.Webhook.Requests;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Converters;

namespace WX.B2C.User.Verification.Facade.Controllers.Webhook.Converters
{
    public class PassFortEventRequestConverter : PolymorphicDeserializer<PassFortEventRequest>
    {
        private static readonly Dictionary<string, Type> KnownTypes = new()
        {
            [Constants.PassFortEventTypes.CheckCompleted] = typeof(PassFortCheckCompletedEventRequest),
            [Constants.PassFortEventTypes.ManualActionRequired] = typeof(PassFortManualActionRequiredEventRequest),
            [Constants.PassFortEventTypes.ProductStatusChanged] = typeof(PassFortProductStatusChangedEventRequest)
        };

        public PassFortEventRequestConverter()
            : base(nameof(PassFortEventRequest.Event).ToLower())
        {
        }

        protected override bool TryGetType(string discriminator, out Type resolvedType)
        {
            if (string.IsNullOrWhiteSpace(discriminator))
                throw new ArgumentNullException(nameof(discriminator));

            if (!KnownTypes.TryGetValue(discriminator, out resolvedType))
                resolvedType = typeof(UnsupportedPassFortEventRequest);

            return true;
        }
    }
}