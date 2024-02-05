namespace WX.B2C.User.Verification.Worker.Jobs.Models.Verification
{
    public enum SurveyCheckStatus
    {
        Requested = 1,
        Assigned = 2,
        OnReview = 3,
        Completed = 4,
        Canceled = 5
    }
}