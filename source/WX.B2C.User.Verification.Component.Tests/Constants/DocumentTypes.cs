namespace WX.B2C.User.Verification.Component.Tests.Constants
{
    internal static class DocumentTypes
    {
        public const string Photo = nameof(Photo);
        public const string Video = nameof(Video);
        public const string W9Form = nameof(W9Form);
        public const string TaxReturn = nameof(TaxReturn);
        public const string CouncilTax = nameof(CouncilTax);
        public const string BankStatement = nameof(BankStatement);
        public const string UtilityBill = nameof(UtilityBill);
        public const string CertificateOfResidency = nameof(CertificateOfResidency);

        public const string DriverLicense = nameof(DriverLicense);
        public const string Passport = nameof(Passport);
        public const string PassportCard = nameof(PassportCard);
        public const string InternationalPassport = nameof(InternationalPassport);
        public const string IdentityCard = nameof(IdentityCard);
        public const string PostalIdentityCard = nameof(PostalIdentityCard);
        public const string SocialSecurityCard = nameof(SocialSecurityCard);
        public const string ResidencePermit = nameof(ResidencePermit);
        public const string WorkPermit = nameof(WorkPermit);
        public const string VoterId = nameof(VoterId);
        public const string Visa = nameof(Visa);
        public const string Other = nameof(Other);
        
        public const string Payslip = nameof(Payslip);
        public const string LetterSalary = nameof(LetterSalary);
        public const string AuditedBankStatement = nameof(AuditedBankStatement);
        public const string CompanyAccounts = nameof(CompanyAccounts);
        public const string LetterRegulatedAccountant = nameof(LetterRegulatedAccountant);
        public const string LetterSolicitor = nameof(LetterSolicitor);
        public const string BusinessSale = nameof(BusinessSale);
        public const string DividendContract = nameof(DividendContract);
        public const string CompanyBankStatement = nameof(CompanyBankStatement);
        public const string InvestCertificates = nameof(InvestCertificates);
        public const string InvestBankStatement = nameof(InvestBankStatement);
        public const string InvestLetterRegulatedAccountant = nameof(InvestLetterRegulatedAccountant);
        public const string ScreenshotSourceWallet = nameof(ScreenshotSourceWallet);
        public const string PensionStatement = nameof(PensionStatement);
        public const string PensionLetterRegulatedAccountant = nameof(PensionLetterRegulatedAccountant);
        public const string PensionLetterAnnuityProvider = nameof(PensionLetterAnnuityProvider);
        public const string PensionBankStatement = nameof(PensionBankStatement);
        public const string PensionSavingsStatement = nameof(PensionSavingsStatement);
        public const string DepositStatement = nameof(DepositStatement);
        public const string DepositEvidence = nameof(DepositEvidence);
        public const string LetterDonor = nameof(LetterDonor);
        public const string InvestmentContractNotes = nameof(InvestmentContractNotes);
        public const string CashInStatement = nameof(CashInStatement);
        
        public static readonly string[] IdentityDocumentTypes =
        {
            Passport,
            DriverLicense,
            PassportCard,
            InternationalPassport,
            IdentityCard,
            PostalIdentityCard,
            SocialSecurityCard,
            ResidencePermit,
            WorkPermit,
            Visa,
            VoterId,
            Other
        };
        public static readonly string[] AddressDocumentTypes =
        {
            BankStatement,
            UtilityBill,
            TaxReturn,
            CouncilTax,
            CertificateOfResidency,
            Other,
        };

        public static readonly string[] ProofOfFundsTypes =
        {
            Payslip,
            LetterSalary,
            AuditedBankStatement,
            CompanyAccounts,
            LetterRegulatedAccountant,
            LetterSolicitor,
            BusinessSale,
            DividendContract,
            CompanyBankStatement,
            InvestCertificates,
            InvestBankStatement,
            InvestLetterRegulatedAccountant,
            ScreenshotSourceWallet,
            PensionStatement,
            PensionLetterRegulatedAccountant,
            PensionLetterAnnuityProvider,
            PensionBankStatement,
            PensionSavingsStatement,
            DepositStatement,
            DepositEvidence,
            LetterDonor,
            InvestmentContractNotes,
            CashInStatement,
            Other
        };
        
        public static readonly string[] SelfieTypes =
        {
            Photo,
            Video
        };

        public static readonly string[] TaxationTypes =
        {
            W9Form
        };

        public static readonly string[] SupportingTypes =
        {
            Other
        };
    }
}
