namespace WX.B2C.User.Verification.Core.Services.Tickets
{
    /// <summary>
    /// TODO later will be a mechanism to detect ticket reason according to policy
    /// </summary>
    internal class TicketReasons
    {
        public const string CheckErrorDetected = "check-error-detected";
        public const string IpMatchFailed = "ip-match-failed";
        public const string UsaIpMatchFailed = "usa-ip-match-failed";
        public const string NameAndDoBDuplication = "name-and-dob-duplication-detected";
        public const string IdDocNumberDuplication = "id-doc-number-duplication-detected";
        public const string PassfortRiskListsScreening = "pass-fort-risk-screening-failed";
        public const string LexisNexisRiskListsScreening = "lexis-nexis-risk-screening-failed";
        public const string UsaOnfidoCheckFailed = "usa-onfido-check-failed";
        public const string InstantIdCheckFailed = "instant-id-check-failed";
        public const string PoIAttemptsExceeded = "poi-attempts-exceeded";

        public const string ProofOfAddressReviewRequired = "poa-doc-verification-needed";
        public const string ProofOfFundsReviewRequired = "sof-doc-verification-needed";
        public const string W9FormReviewRequired = "w-9-form-review-needed";

        public const string UsaEddCheckListReviewRequired = "usa-edd-check-list-review-needed";
        public const string AccountRefreshAlert = "account-alert-created";
    }
}