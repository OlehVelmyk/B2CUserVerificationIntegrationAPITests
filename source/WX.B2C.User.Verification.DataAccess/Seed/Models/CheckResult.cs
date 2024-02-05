namespace WX.B2C.User.Verification.DataAccess.Seed.Models
{
    internal class CheckResult
    {
        public CheckResultType Type { get; set; }

        public object Result { get; set; }

        public object Condition { get; set; }
    }
}