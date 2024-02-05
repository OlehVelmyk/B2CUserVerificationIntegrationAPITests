using System;
using Castle.Core.Internal;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.DataAccess.Entities;

namespace WX.B2C.User.Verification.DataAccess.Mappers
{
    internal interface IVerificationDetailsMapper
    {
        VerificationDetailsDto Map(VerificationDetails entity);

        VerificationDetails Map(VerificationDetailsDto verificationDetailsDto);

        void Update(VerificationDetailsDto dto, VerificationDetails entity);
    }

    internal class VerificationDetailsMapper : IVerificationDetailsMapper
    {
        public VerificationDetailsDto Map(VerificationDetails entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new VerificationDetailsDto
            {
                UserId = entity.UserId,
                IpAddress = entity.IpAddress,
                TaxResidence = entity.TaxResidence,
                IdDocumentNumber = MapIdDocumentNumber(entity),
                Tin = entity.Tin,
                RiskLevel = entity.RiskLevel,
                Nationality = entity.Nationality,
                IsPep = entity.IsPep,
                IsSanctioned = entity.IsSanctioned,
                IsAdverseMedia = entity.IsAdverseMedia,
                Turnover = entity.Turnover,
                PoiIssuingCountry = entity.PoiIssuingCountry,
                PlaceOfBirth = entity.PlaceOfBirth,
                ComprehensiveIndex = entity.ComprehensiveIndex,
                IsIpMatched = entity.IsIpMatched,
                ResolvedCountryCode = entity.ResolvedCountryCode
            };
        }

        public VerificationDetails Map(VerificationDetailsDto verificationDetailsDto)
        {
            if (verificationDetailsDto == null)
                throw new ArgumentNullException(nameof(verificationDetailsDto));

            var verificationDetails = new VerificationDetails
            {
                UserId = verificationDetailsDto.UserId,
            };
            Update(verificationDetailsDto, verificationDetails);
            return verificationDetails;
        }

        public void Update(VerificationDetailsDto dto, VerificationDetails entity)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            
            entity.IpAddress = dto.IpAddress;
            entity.TaxResidence = dto.TaxResidence;
            entity.Tin = dto.Tin;
            entity.RiskLevel = dto.RiskLevel;
            entity.Nationality = dto.Nationality;
            entity.IsPep = dto.IsPep;
            entity.IsSanctioned = dto.IsSanctioned;
            entity.IsAdverseMedia = dto.IsAdverseMedia;
            entity.Turnover = dto.Turnover;
            entity.PoiIssuingCountry = dto.PoiIssuingCountry;
            entity.PlaceOfBirth = dto.PlaceOfBirth;
            entity.ComprehensiveIndex = dto.ComprehensiveIndex;
            entity.IsIpMatched = dto.IsIpMatched;
            entity.ResolvedCountryCode = dto.ResolvedCountryCode;
            entity.IdDocumentNumber = dto.IdDocumentNumber?.Number;
            entity.IdDocumentNumberType = dto.IdDocumentNumber?.Type;
        }

        private static IdDocumentNumberDto MapIdDocumentNumber(VerificationDetails entity)
        {
            if (entity.IdDocumentNumber.IsNullOrEmpty())
                return null;
            
            return new IdDocumentNumberDto 
            { 
                Number = entity.IdDocumentNumber,
                Type = entity.IdDocumentNumberType 
            };
        }
    }
}