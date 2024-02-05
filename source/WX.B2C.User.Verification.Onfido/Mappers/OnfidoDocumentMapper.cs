using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Onfido.Constants;

namespace WX.B2C.User.Verification.Onfido.Mappers
{
    internal interface IOnfidoDocumentMapper
    {
        string Map(string documentType);

        DocumentType MapFromOnfido(string documentType);
    }

    internal class OnfidoDocumentMapper : IOnfidoDocumentMapper
    {
        public string Map(string documentType) =>
            documentType switch
            {
                nameof(IdentityDocumentType.DriverLicense) => OnfidoDocumentType.DrivingLicence,
                nameof(IdentityDocumentType.Passport) => OnfidoDocumentType.Passport,
                nameof(IdentityDocumentType.IdentityCard) => OnfidoDocumentType.NationalIdentityCard,
                nameof(AddressDocumentType.BankStatement) => OnfidoDocumentType.BankBuildingSocietyStatement,
                nameof(AddressDocumentType.UtilityBill) => OnfidoDocumentType.UtilityBill,
                nameof(AddressDocumentType.CouncilTax) => OnfidoDocumentType.CouncilTax,
                nameof(AddressDocumentType.TaxReturn) => OnfidoDocumentType.BenefitLetters,
                _ => OnfidoDocumentType.Unknown
            };

        public DocumentType MapFromOnfido(string documentType) =>
            documentType switch
            {
                OnfidoDocumentType.DrivingLicence => IdentityDocumentType.DriverLicense,
                OnfidoDocumentType.Passport => IdentityDocumentType.Passport,
                OnfidoDocumentType.NationalIdentityCard => IdentityDocumentType.IdentityCard,
                // TODO: Why address document types appears in Onfido?
                OnfidoDocumentType.BankBuildingSocietyStatement => AddressDocumentType.BankStatement,
                OnfidoDocumentType.UtilityBill => AddressDocumentType.UtilityBill,
                OnfidoDocumentType.CouncilTax => AddressDocumentType.CouncilTax,
                OnfidoDocumentType.BenefitLetters => AddressDocumentType.TaxReturn,
                _ => IdentityDocumentType.Other
            };
    }
}
