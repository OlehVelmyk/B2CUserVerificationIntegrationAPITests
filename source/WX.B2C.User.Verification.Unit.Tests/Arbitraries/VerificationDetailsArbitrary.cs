using System;
using FsCheck;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Domain.Profile;
using WX.B2C.User.Verification.Unit.Tests.Arbitraries.Generators;
using WX.B2C.User.Verification.Unit.Tests.Arbitraries.TestData;

namespace WX.B2C.User.Verification.Unit.Tests.Arbitraries
{
    public class VerificationDetailsArbitrary : Arbitrary<VerificationDetailsDto>
    {
        public static Arbitrary<VerificationDetailsDto> Create()
        {
            return new VerificationDetailsArbitrary();
        }

        public override Gen<VerificationDetailsDto> Generator =>
            from userId in Arb.Generate<Guid>()
            from ipAddress in Arb.Generate<IPv4Address>()
            from idDocNumber in StringGenerators.NotEmpty(4, 10)
            from taxResidence in Arb.Generate<TaxResidence>()
            from tin in Arb.Generate<TinDto>().OrNull()
            from riskLevel in Arb.Generate<RiskLevel?>().Or(Gen.Constant<RiskLevel?>(null))
            from isPep in Arb.Generate<bool?>().Or(Gen.Constant<bool?>(null))
            from isSanctioned in Arb.Generate<bool?>().Or(Gen.Constant<bool?>(null))
            from isAdverseMedia in Arb.Generate<bool?>().Or(Gen.Constant<bool?>(null))
            select new VerificationDetailsDto
            {
                UserId = userId,
                IpAddress = ipAddress.Item.ToString(),
                IdDocumentNumber = new IdDocumentNumberDto { Number = idDocNumber, Type = IdentityDocumentType.Passport },
                TaxResidence = taxResidence.Value,
                Tin = tin,
                RiskLevel = riskLevel,
                IsPep = isPep,
                IsSanctioned = isSanctioned,
                IsAdverseMedia = isAdverseMedia
            };
    }

    public class NotEmptyVerificationDetailsArbitrary : Arbitrary<NotEmptyVerificationDetails>
    {
        public static Arbitrary<NotEmptyVerificationDetails> Create()
        {
            return new NotEmptyVerificationDetailsArbitrary();
        }

        public override Gen<NotEmptyVerificationDetails> Generator =>
            from userId in Arb.Generate<Guid>()
            from ipAddress in Arb.Generate<IPv4Address>()
            from idDocNumber in StringGenerators.NotEmpty(4, 10)
            from taxResidence in Arb.Generate<TaxResidence>().Where(residence => residence.HasValue)
            from tin in Arb.Generate<TinDto>()
            select new NotEmptyVerificationDetails
            {
                UserId = userId,
                IpAddress = ipAddress.Item.ToString(),
                IdDocumentNumber = new IdDocumentNumberDto { Number = idDocNumber, Type = IdentityDocumentType.Passport },
                TaxResidence = taxResidence.Value,
                Tin = tin
            };
    }

    public class NotEmptyVerificationDetails
    {
        public static implicit operator VerificationDetailsDto(NotEmptyVerificationDetails verificationDetails)
        {
            return new VerificationDetailsDto
            {
                IpAddress = verificationDetails.IpAddress,
                IdDocumentNumber = verificationDetails.IdDocumentNumber,
                TaxResidence = verificationDetails.TaxResidence,
                Tin = verificationDetails.Tin,
                UserId = verificationDetails.UserId,
            };
        }

        public Guid UserId { get; set; }

        public string IpAddress { get; set; }

        public string[] TaxResidence { get; set; }

        public IdDocumentNumberDto IdDocumentNumber { get; set; }

        public TinDto Tin { get; set; }
    }
}