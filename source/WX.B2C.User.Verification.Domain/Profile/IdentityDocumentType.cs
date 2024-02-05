using WX.Domain;

namespace WX.B2C.User.Verification.Domain.Enums
{
    public abstract class DocumentType : Enumeration<DocumentType>
    {
        protected DocumentType(string value, string description)
            : base(value)
        {
            Description = description;
        }

        public string Description { get; private set; }
    }

    public class IdentityDocumentType : DocumentType
    {
        public IdentityDocumentType(string value, string description)
            : base(value, description)
        {
        }

        public static IdentityDocumentType Passport => new(nameof(Passport), "Passport");

        public static IdentityDocumentType DriverLicense => new(nameof(DriverLicense), "Driver license");

        public static IdentityDocumentType IdentityCard => new(nameof(IdentityCard), "Identity card");

        public static IdentityDocumentType BirthCertificate => new(nameof(BirthCertificate), "Birth certificate");

        public static IdentityDocumentType Other => new(nameof(Other), "Other");
    }

    public class AddressDocumentType : DocumentType
    {
        public AddressDocumentType(string value, string description)
            : base(value, description)
        {
        }

        public static AddressDocumentType CertificateOfResidency => new(nameof(CertificateOfResidency), "Certificate of residency");

        public static AddressDocumentType BankStatement => new(nameof(BankStatement), "Bank statement");

        public static AddressDocumentType UtilityBill => new(nameof(UtilityBill), "Utility bill");

        public static AddressDocumentType CouncilTax => new(nameof(CouncilTax), "Council tax");

        public static AddressDocumentType TaxReturn => new(nameof(TaxReturn), "Tax return");

        public static AddressDocumentType Other => new(nameof(Other), "Other");
    }

    public class SelfieDocumentType : DocumentType
    {
        public SelfieDocumentType(string value, string description)
            : base(value, description)
        {
        }

        public static SelfieDocumentType Photo => new(nameof(Photo), "Photo");

        public static SelfieDocumentType Video => new(nameof(Video), "Video");
    }

    public class TaxationDocumentType : DocumentType
    {
        public TaxationDocumentType(string value, string description)
            : base(value, description)
        {
        }

        public static TaxationDocumentType W9Form => new(nameof(W9Form), "W9 Form");
    }

    public class FundsDocumentType : DocumentType
    {
        public FundsDocumentType(string value, string description)
            : base(value, description)
        {
        }

        public static FundsDocumentType Payslip => new(nameof(Payslip), "Payslip");
        public static FundsDocumentType LetterSalary => new(nameof(LetterSalary), "Letter salary");
        public static FundsDocumentType AuditedBankStatement => new(nameof(AuditedBankStatement), "Audited bank statement");
        public static FundsDocumentType CompanyAccounts => new(nameof(CompanyAccounts), "Company accounts");
        public static FundsDocumentType LetterRegulatedAccountant => new(nameof(LetterRegulatedAccountant), "Letter regulated accountant");
        public static FundsDocumentType LetterSolicitor => new(nameof(LetterSolicitor), "Letter solicitor");
        public static FundsDocumentType BusinessSale => new(nameof(BusinessSale), "Business sale");
        public static FundsDocumentType DividendContract => new(nameof(DividendContract), "Dividend contract");
        public static FundsDocumentType CompanyBankStatement => new(nameof(CompanyBankStatement), "Company bank statement");
        public static FundsDocumentType InvestCertificates => new(nameof(InvestCertificates), "Invest certificates");
        public static FundsDocumentType InvestBankStatement => new(nameof(InvestBankStatement), "Invest bank statement");
        public static FundsDocumentType InvestLetterRegulatedAccountant => new(nameof(InvestLetterRegulatedAccountant), "Invest letter regulated accountant");
        public static FundsDocumentType ScreenshotSourceWallet => new(nameof(ScreenshotSourceWallet), "Screenshot source wallet");
        public static FundsDocumentType PensionStatement => new(nameof(PensionStatement), "Pension statement");
        public static FundsDocumentType PensionLetterRegulatedAccountant => new(nameof(PensionLetterRegulatedAccountant), "Pension letter regulated accountant");
        public static FundsDocumentType PensionLetterAnnuityProvider => new(nameof(PensionLetterAnnuityProvider), "Pension letter annuity provider");
        public static FundsDocumentType PensionBankStatement => new(nameof(PensionBankStatement), "Pension bank statement");
        public static FundsDocumentType PensionSavingsStatement => new(nameof(PensionSavingsStatement), "Pension savings statement");
        public static FundsDocumentType DepositStatement => new(nameof(DepositStatement), "Deposit statement");
        public static FundsDocumentType DepositEvidence => new(nameof(DepositEvidence), "Deposit evidence");
        public static FundsDocumentType LetterDonor => new(nameof(LetterDonor), "Letter donor");
        public static FundsDocumentType InvestmentContractNotes => new(nameof(InvestmentContractNotes), "Investment contract notes");
        public static FundsDocumentType CashInStatement => new(nameof(CashInStatement), "Cash in statement");
        public static FundsDocumentType Other => new(nameof(Other), "Other");
        
    }

    public class SupportingDocumentType : DocumentType
    {
        public SupportingDocumentType(string value, string description)
            : base(value, description)
        {
        }

        public static SupportingDocumentType Other => new(nameof(Other), "Other");
    }
}
