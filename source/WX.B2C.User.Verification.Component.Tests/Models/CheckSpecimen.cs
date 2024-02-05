using System;
using System.Collections.Generic;

namespace WX.B2C.User.Verification.Component.Tests.Models
{
    internal class CheckSpecimen
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Type { get; set; }

        public string State { get; set; }

        public string Result { get; set; }

        public string Provider { get; set; }

        public Guid VariantId { get; set; }

        public string ExternalId { get; set; }

        public IDictionary<string, object> ExternalData { get; set; }
    }
}
