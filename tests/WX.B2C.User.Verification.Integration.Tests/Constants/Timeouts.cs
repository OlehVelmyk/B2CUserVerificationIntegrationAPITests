namespace WX.B2C.User.Verification.Integration.Tests.Constants;

public static class Timeouts
{
    public const int DefaultRetryAttempts = 50;
    public static readonly TimeSpan VerificationWaitTimeout = TimeSpan.FromSeconds(80);

    public static TimeSpan GetTimeout(int retryAttempt) => ApplyMaxTimeoutConstraint(TimeSpan.FromMilliseconds(retryAttempt * 500));

    private static TimeSpan ApplyMaxTimeoutConstraint(TimeSpan original) =>
        original <= TimeSpan.FromSeconds(5) ? original : TimeSpan.FromSeconds(5);
}
