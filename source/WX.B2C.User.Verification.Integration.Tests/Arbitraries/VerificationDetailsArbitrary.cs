using System;
using FsCheck;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Domain.Profile;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries.Generators;
using WX.B2C.User.Verification.Integration.Tests.Models;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries.Utilities;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries
{
    internal class VerificationDetailsArbitrary : Arbitrary<VerificationDetailsDto>
    {
        public static Arbitrary<VerificationDetailsDto> Create() => new VerificationDetailsArbitrary();

        public override Gen<VerificationDetailsDto> Generator =>
            from userId in Arb.Generate<Guid>()
            from ipAddress in Arb.Generate<IPv4Address>()
            from number in StringGenerators.NotEmpty(4, 10)
            from taxResidence in Arb.Generate<TaxResidence>()
            from tin in Arb.Generate<TinDto>().OrNull()
            from riskLevel in Arb.Generate<RiskLevel?>().OrNull()
            from isPep in Arb.Generate<bool?>().OrNull()
            from isSanctioned in Arb.Generate<bool?>().OrNull()
            from isAdverseMedia in Arb.Generate<bool?>().OrNull()
            select new VerificationDetailsDto
            {
                UserId = userId,
                IpAddress = ipAddress.Item.ToString(),
                IdDocumentNumber = new IdDocumentNumberDto() { Number = number, Type = IdentityDocumentType.Passport },
                TaxResidence = taxResidence.Value,
                Tin = new TinDto() { Number = number, Type = Core.Contracts.Enum.TinType.SSN },
                RiskLevel = riskLevel,
                IsPep = isPep,
                IsSanctioned = isSanctioned,
                IsAdverseMedia = isAdverseMedia
            };
    }
}