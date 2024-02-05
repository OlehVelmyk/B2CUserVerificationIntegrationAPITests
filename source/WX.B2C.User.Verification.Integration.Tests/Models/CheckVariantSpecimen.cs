using System;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Domain.Checks;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Models
{
    public class CheckVariantSpecimen
    {
        public Guid Id { get; set; }

        public CheckType Type { get; set; }

        public CheckProviderType Provider { get; set; }
        
        public int? MaxAttempts { get; set; }
    }
}
