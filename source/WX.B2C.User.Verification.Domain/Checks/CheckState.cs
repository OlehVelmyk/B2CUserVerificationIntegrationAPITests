namespace WX.B2C.User.Verification.Domain.Models
{
    public enum CheckState
    {
        Pending = 1,
        Running = 2,
        Complete = 3,
        Error = 4,
        Cancelled = 5
    }
}