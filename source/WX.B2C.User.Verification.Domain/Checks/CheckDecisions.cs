namespace WX.B2C.User.Verification.Domain.Checks
{
    public static class CheckDecisions
    {
        public const string Resubmit = nameof(Resubmit);
        public const string Consider = nameof(Consider);
        public const string Fraud = nameof(Fraud);
        public const string PotentialFraud = nameof(PotentialFraud);
        public const string InstantIdClosing = nameof(InstantIdClosing);
        public const string DuplicateAccount = "Duplicate account";
    }
}
