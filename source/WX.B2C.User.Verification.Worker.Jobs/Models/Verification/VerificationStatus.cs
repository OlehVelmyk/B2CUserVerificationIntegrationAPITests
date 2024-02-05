namespace WX.B2C.User.Verification.Worker.Jobs.Models.Verification
{
    public enum VerificationStatus
    {
        Unverified = 0,
        Pending = 1,
        Verified = 2,
        AdditionalDocumentsRequired = 3,
        AddressUnverified = 4,
        Rejected = 5,
        Fraud = 6,
        AdditionalInfoRequired = 7,
        PFraud = 8
    }
}