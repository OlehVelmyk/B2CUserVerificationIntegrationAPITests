using System;
using Optional;
using WX.B2C.User.Verification.Facade.Controllers.Public.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Public.Requests;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Mappers
{
    public interface IVerificationDetailsMapper
    {
        Core.Contracts.Dtos.Profile.VerificationDetailsPatch Map(UpdateVerificationDetailsRequest request);

        VerificationDetailsDto Map(Core.Contracts.Dtos.Profile.VerificationDetailsDto verificationDetails);
    }

    internal class VerificationDetailsMapper : IVerificationDetailsMapper
    {
        public Core.Contracts.Dtos.Profile.VerificationDetailsPatch Map(UpdateVerificationDetailsRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return new Core.Contracts.Dtos.Profile.VerificationDetailsPatch
            {
                TaxResidence = request.TaxResidence.SomeNotNull(),
                Tin = request.Tin.SomeNotNull().Map(Map)
            };
        }

        public VerificationDetailsDto Map(Core.Contracts.Dtos.Profile.VerificationDetailsDto verificationDetails)
        {
            if (verificationDetails == null)
                throw new ArgumentNullException(nameof(verificationDetails));

            return new VerificationDetailsDto
            {
                TaxResidence = verificationDetails.TaxResidence,
                IdDocumentNumber = Map(verificationDetails.IdDocumentNumber),
                Tin = Map(verificationDetails.Tin)
            };
        }

        private static Core.Contracts.Dtos.Profile.TinDto Map(TinDto tin)
        {
            if (tin == null)
                throw new ArgumentNullException(nameof(tin));

            return new Core.Contracts.Dtos.Profile.TinDto
            {
                Number = tin.Number,
                Type = tin.Type
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