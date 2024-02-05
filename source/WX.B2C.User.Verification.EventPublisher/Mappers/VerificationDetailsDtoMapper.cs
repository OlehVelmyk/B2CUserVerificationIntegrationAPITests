using System;
using WX.B2C.User.Verification.Events.Dtos;
using WX.B2C.User.Verification.Events.Enums;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.EventPublisher.Mappers
{
    internal interface IVerificationDetailsDtoMapper
    {
        VerificationDetailsDto Map(Core.Contracts.Dtos.Profile.VerificationDetailsDto verificationDetailsDto);
    }

    internal class VerificationDetailsDtoMapper : IVerificationDetailsDtoMapper
    {
        public VerificationDetailsDto Map(Core.Contracts.Dtos.Profile.VerificationDetailsDto verificationDetails)
        {
            if (verificationDetails == null)
                throw new ArgumentNullException(nameof(verificationDetails));

            return new VerificationDetailsDto
            {
                ComprehensiveIndex = verificationDetails.ComprehensiveIndex,
                IsAdverseMedia = verificationDetails.IsAdverseMedia,
                IsIpMatched = verificationDetails.IsIpMatched,
                IsPep = verificationDetails.IsPep,
                IsSanctioned = verificationDetails.IsSanctioned,
                Nationality = verificationDetails.Nationality,
                PlaceOfBirth = verificationDetails.PlaceOfBirth,
                PoiIssuingCountry = verificationDetails.PoiIssuingCountry,
                ResolvedCountryCode = verificationDetails.ResolvedCountryCode,
                Tin = verificationDetails.Tin == null ? null : new TinDto
                {
                    Type = verificationDetails.Tin.Type.To<TinType>(),
                    Number = verificationDetails.Tin.Number
                }
            };
        }
    }
}