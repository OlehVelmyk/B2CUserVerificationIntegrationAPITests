using System;
using Optional;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers
{
    public interface IVerificationDetailsMapper
    {
        VerificationDetailsDto SafeMap(Core.Contracts.Dtos.Profile.VerificationDetailsDto verificationDetails);

        Core.Contracts.Dtos.Profile.VerificationDetailsPatch Map(UpdateVerificationDetailsRequest request);

        TinDto SafeMap(Core.Contracts.Dtos.Profile.TinDto tin);

        IdDocumentNumberDto SafeMap(Core.Contracts.Dtos.Profile.IdDocumentNumberDto documentNumber);
    }

    internal class VerificationDetailsMapper : IVerificationDetailsMapper
    {
        public VerificationDetailsDto SafeMap(Core.Contracts.Dtos.Profile.VerificationDetailsDto verificationDetails)
        {
            if (verificationDetails == null) return null;

            return new VerificationDetailsDto
            {
                TaxResidence = verificationDetails.TaxResidence,
                IdDocumentNumber = SafeMap(verificationDetails.IdDocumentNumber),
                Tin = SafeMap(verificationDetails.Tin),
                VerificationIpAddress = verificationDetails.IpAddress,
                IsAdverseMedia = verificationDetails.IsAdverseMedia,
                IsPep = verificationDetails.IsPep,
                IsSanctioned = verificationDetails.IsSanctioned,
                Turnover = verificationDetails.Turnover,
                PoiIssuingCountry = verificationDetails.PoiIssuingCountry,
                PlaceOfBirth = verificationDetails.PlaceOfBirth,
                ComprehensiveIndex = verificationDetails.ComprehensiveIndex,
                IsIpMatched = verificationDetails.IsIpMatched,
                ResolvedCountryCode = verificationDetails.ResolvedCountryCode
            };
        }

        public Core.Contracts.Dtos.Profile.VerificationDetailsPatch Map(UpdateVerificationDetailsRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return new Core.Contracts.Dtos.Profile.VerificationDetailsPatch
            {
                TaxResidence = request.TaxResidence.SomeNotNull(),
                IdDocumentNumber = SafeMap(request.IdDocumentNumber).SomeNotNull(),
                Tin = SafeMap(request.Tin).SomeNotNull(),
                IsAdverseMedia = request.IsAdverseMedia.SomeNotNull(),
                IsPep = request.IsPep.SomeNotNull(),
                IsSanctioned = request.IsSanctioned.SomeNotNull()
            };
        }

        public IdDocumentNumberDto SafeMap(Core.Contracts.Dtos.Profile.IdDocumentNumberDto documentNumber)
        {
            if (documentNumber == null)
                return null;

            return new IdDocumentNumberDto
            {
                Number = documentNumber.Number,
                Type = documentNumber.Type
            };
        }

        public TinDto SafeMap(Core.Contracts.Dtos.Profile.TinDto tin)
        {
            if (tin == null)
                return null;

            return new TinDto
            {
                Number = tin.Number,
                Type = tin.Type
            };
        }

        private static Core.Contracts.Dtos.Profile.IdDocumentNumberDto SafeMap(IdDocumentNumberDto requestIdDocumentNumber)
        {
            if (requestIdDocumentNumber == null)
                return null;

            return new Core.Contracts.Dtos.Profile.IdDocumentNumberDto
            {
                Number = requestIdDocumentNumber.Number,
                Type = requestIdDocumentNumber.Type
            };
        }

        private static Core.Contracts.Dtos.Profile.TinDto SafeMap(TinDto tin)
        {
            if (tin == null)
                return null;

            return new Core.Contracts.Dtos.Profile.TinDto
            {
                Number = tin.Number,
                Type = tin.Type
            };
        }
    }
}