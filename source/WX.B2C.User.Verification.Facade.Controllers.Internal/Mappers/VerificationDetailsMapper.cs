using System;
using WX.B2C.User.Verification.Facade.Controllers.Internal.Dtos;

namespace WX.B2C.User.Verification.Facade.Controllers.Internal.Mappers
{
    public interface IVerificationDetailsMapper
    {
        VerificationDetailsDto ToDto(Core.Contracts.Dtos.Profile.VerificationDetailsDto verificationDetails);
    }

    internal class VerificationDetailsMapper : IVerificationDetailsMapper
    {
        public VerificationDetailsDto ToDto(Core.Contracts.Dtos.Profile.VerificationDetailsDto verificationDetails)
        {
            if (verificationDetails == null)
                throw new ArgumentNullException(nameof(verificationDetails));

            return new VerificationDetailsDto
            {
                TaxResidence = verificationDetails.TaxResidence,
                IdDocumentNumber = Map(verificationDetails.IdDocumentNumber),
                Tin = Map(verificationDetails.Tin),
                VerificationIpAddress = verificationDetails.IpAddress,
                IsAdverseMedia = verificationDetails.IsAdverseMedia,
                IsPep = verificationDetails.IsPep,
                IsSanctioned = verificationDetails.IsSanctioned,
                Turnover = verificationDetails.Turnover,
                Nationality = verificationDetails.Nationality,
                PoiIssuingCountry = verificationDetails.PoiIssuingCountry,
                PlaceOfBirth = verificationDetails.PlaceOfBirth,
                ComprehensiveIndex = verificationDetails.ComprehensiveIndex,
                IsIpMatched = verificationDetails.IsIpMatched,
                ResolvedCountryCode = verificationDetails.ResolvedCountryCode,
                RiskLevel = verificationDetails.RiskLevel
            };
        }

        private static IdDocumentNumberDto Map(Core.Contracts.Dtos.Profile.IdDocumentNumberDto documentNumber)
        {
            if (documentNumber == null)
                return null;

            return new IdDocumentNumberDto
            {
                Number = documentNumber.Number,
                Type = documentNumber.Type
            };
        }

        private static TinDto Map(Core.Contracts.Dtos.Profile.TinDto tin)
        {
            if (tin == null)
                return null;

            return new TinDto
            {
                Number = tin.Number,
                Type = tin.Type
            };
        }
    }
}