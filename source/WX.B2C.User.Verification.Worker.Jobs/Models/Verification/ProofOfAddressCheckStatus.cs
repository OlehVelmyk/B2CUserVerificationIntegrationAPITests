namespace WX.B2C.User.Verification.Worker.Jobs.Models.Verification
{
    public enum ProofOfAddressCheckStatus
    {
        Requested = 1,
        Required = 2,
        Completed = 3,
        Failed = 4,
        Canceled = 5
    }
}