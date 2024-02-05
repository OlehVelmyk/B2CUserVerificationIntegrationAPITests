namespace WX.B2C.User.Verification.Core.Contracts.Triggers
{
    public static class JobConstants
    {
        public const string ScheduledTriggerJobName = "scheduled-trigger";
        public const string TriggerId = nameof(TriggerId);
        public const string UserId = nameof(UserId);

        public const string UserReminderJob = "user-reminder";
        public const string ReminderId = nameof(ReminderId);
    }
}