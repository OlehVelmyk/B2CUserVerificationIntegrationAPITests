using System;
using FsCheck;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.Integration.Tests.Builders
{
    public class VerificationDetailsDtoBuilder
    {
        private readonly VerificationDetailsDto _verificationDetailsDto;

        private VerificationDetailsDtoBuilder(VerificationDetailsDto verificationDetailsDto)
        {
            if (verificationDetailsDto == null)
                throw new ArgumentNullException(nameof(verificationDetailsDto));

            _verificationDetailsDto = new VerificationDetailsDto
            {
                IpAddress = verificationDetailsDto.IpAddress,
                IdDocumentNumber = verificationDetailsDto.IdDocumentNumber,
                TaxResidence = verificationDetailsDto.TaxResidence,
                UserId = verificationDetailsDto.UserId,
            };
        }

        public static VerificationDetailsDtoBuilder WithInitial(VerificationDetailsDto verificationDetailsDto)
        {
            return new VerificationDetailsDtoBuilder(verificationDetailsDto);
        }

        public VerificationDetailsDtoBuilder WithIpAddress(IPv4Address ipAddress)
        {
            _verificationDetailsDto.IpAddress = ipAddress.Item.ToString();
            return this;
        }

        public VerificationDetailsDtoBuilder WithTaxResidence(string[] taxResidence)
        {
            _verificationDetailsDto.TaxResidence = taxResidence;
            return this;
        }

        public VerificationDetailsDtoBuilder WithIdDocNumber(string idDocNumber, IdentityDocumentType documentType)
        {
            _verificationDetailsDto.IdDocumentNumber = new IdDocumentNumberDto { Number = idDocNumber, Type = documentType };
            return this;
        }

        public VerificationDetailsDto Build()
        {
            return _verificationDetailsDto;
        }
    }
}