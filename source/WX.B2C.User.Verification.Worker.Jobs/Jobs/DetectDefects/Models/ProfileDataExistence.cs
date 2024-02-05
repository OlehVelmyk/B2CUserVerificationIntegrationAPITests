namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Models
{
    internal class ProfileDataExistence
    {
        public bool FullName { get; set; }

        public bool DateOfBirth { get; set; }

        public bool Address { get; set; }

        public bool IpAddress { get; set; }

        public bool TaxResidence { get; set; }

        public bool IdDocumentNumber { get; set; }

        public bool IdDocumentNumberType { get; set; }

        public bool Tin { get; set; }

        public bool Nationality { get; set; }

        public bool RiskLevel { get; set; }

        public bool IdentityDocuments { get; set; }

        public bool AddressDocuments { get; set; }

        public bool ProofOfFundsDocuments { get; set; }

        public bool W9Form { get; set; }
    }
}