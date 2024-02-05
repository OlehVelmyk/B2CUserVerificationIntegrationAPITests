using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.DataAccess
{
    internal class HardcodedDocumentTypeProvider : IDocumentTypeProvider
    {
        private static readonly IDictionary<DocumentCategory, IEnumerable<DocumentType>> DocumentTypes =
            new Dictionary<DocumentCategory, IEnumerable<DocumentType>>
            {
                [DocumentCategory.ProofOfIdentity] = ProofOfIdentityTypes(),
                [DocumentCategory.ProofOfAddress] = ProofOfAddressTypes(),
                [DocumentCategory.ProofOfFunds] = ProofOfFundsTypes(),
                [DocumentCategory.Selfie] = SelfieTypes(),
                [DocumentCategory.Taxation] = TaxationTypes(),
                [DocumentCategory.Supporting] = SupportingTypes()
            };

        public IDictionary<DocumentCategory, IEnumerable<DocumentType>> Get()
        {
            return DocumentTypes;
        }

        public IEnumerable<DocumentType> Get(DocumentCategory category)
        {
            if (!DocumentTypes.TryGetValue(category, out var documentTypes))
                throw new ArgumentOutOfRangeException(nameof(category), category, "Unsupported document category.");

            return documentTypes;
        }

        public bool IsValid(DocumentCategory category, string documentType)
        {
            if (!DocumentTypes.TryGetValue(category, out var documentTypes))
                throw new ArgumentOutOfRangeException(nameof(category), category, "Unsupported document category.");

            return documentTypes.Any(type => type == documentType);
        }

        public DocumentType Get(DocumentCategory category, string type)
        {
            var categoryTypes = Get(category);
            return categoryTypes.Single(x => x == type);
        }

        private static IEnumerable<DocumentType> ProofOfIdentityTypes()
        {
            yield return IdentityDocumentType.Passport;
            yield return IdentityDocumentType.DriverLicense;
            yield return IdentityDocumentType.IdentityCard;
            yield return IdentityDocumentType.Other;
            yield return new IdentityDocumentType("InternationalPassport", "International passport");
            yield return new IdentityDocumentType("PostalIdentityCard", "Postal identity card");
            yield return new IdentityDocumentType("SocialSecurityCard", "Social security card");
            yield return new IdentityDocumentType("VoterId", "VoterId");
            yield return new IdentityDocumentType("PassportCard", "Passport card");
            yield return new IdentityDocumentType("ResidencePermit", "Residence permit");
            yield return new IdentityDocumentType("WorkPermit", "Work permit");
            yield return new IdentityDocumentType("Visa", "Visa");
        }

        private static IEnumerable<AddressDocumentType> ProofOfAddressTypes()
        {
            yield return AddressDocumentType.BankStatement;
            yield return AddressDocumentType.UtilityBill;
            yield return AddressDocumentType.TaxReturn;
            yield return AddressDocumentType.CouncilTax;
            yield return AddressDocumentType.CertificateOfResidency;
            yield return AddressDocumentType.Other;
        }

        private static IEnumerable<FundsDocumentType> ProofOfFundsTypes()
        {
            yield return FundsDocumentType.Payslip;
            yield return FundsDocumentType.LetterSalary;
            yield return FundsDocumentType.AuditedBankStatement;
            yield return FundsDocumentType.CompanyAccounts;
            yield return FundsDocumentType.LetterRegulatedAccountant;
            yield return FundsDocumentType.LetterSolicitor;
            yield return FundsDocumentType.BusinessSale;
            yield return FundsDocumentType.DividendContract;
            yield return FundsDocumentType.CompanyBankStatement;
            yield return FundsDocumentType.InvestCertificates;
            yield return FundsDocumentType.InvestBankStatement;
            yield return FundsDocumentType.InvestLetterRegulatedAccountant;
            yield return FundsDocumentType.ScreenshotSourceWallet;
            yield return FundsDocumentType.PensionStatement;
            yield return FundsDocumentType.PensionLetterRegulatedAccountant;
            yield return FundsDocumentType.PensionLetterAnnuityProvider;
            yield return FundsDocumentType.PensionBankStatement;
            yield return FundsDocumentType.PensionSavingsStatement;
            yield return FundsDocumentType.DepositStatement;
            yield return FundsDocumentType.DepositEvidence;
            yield return FundsDocumentType.LetterDonor;
            yield return FundsDocumentType.InvestmentContractNotes;
            yield return FundsDocumentType.CashInStatement;
            yield return FundsDocumentType.Other;
        }

        private static IEnumerable<DocumentType> SelfieTypes()
        {
            yield return SelfieDocumentType.Photo;
            yield return SelfieDocumentType.Video;
        }

        private static IEnumerable<DocumentType> TaxationTypes()
        {
            yield return TaxationDocumentType.W9Form;
        }

        private static IEnumerable<DocumentType> SupportingTypes()
        {
            yield return SupportingDocumentType.Other;
        }
    }
}