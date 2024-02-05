using System;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.ProviderModels
{
        internal class Profile
        {
            public Guid UserId { get; set; }

            public Guid ApplicationId { get; set; }

            public Guid PolicyId { get; set; }

            public ApplicationState ApplicationState { get; set; }

            public bool FullName { get; set; }

            public bool DateOfBirth { get; set; }

            public bool Address { get; set; }

            public bool IpAddress { get; set; }

            public bool TaxResidence { get; set; }

            public bool IdDocumentNumber { get; set; }

            public bool IdDocumentNumberType { get; set; }

            public bool Tin { get; set; }

            public bool Nationality { get; set; }

            public bool RiskLevel { get; set; }
        }
}