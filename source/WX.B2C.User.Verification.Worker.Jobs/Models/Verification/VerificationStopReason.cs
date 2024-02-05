using System;

namespace WX.B2C.User.Verification.Worker.Jobs.Models.Verification
{
    [Flags]
    public enum VerificationStopReason
    {
        Undefined = 0,
        UsaCitizenFormSubmission = 2 << 0,
        UsaCitizenFormDone = 2 << 1,
        UsaCitizenFormReviewed = 2 << 2,

        OnboardingSurveySubmission = 2 << 3,
        OnboardingSurveyDone = 2 << 4,

        PepSurveySubmission = 2 << 5,
        PepSurveyDone = 2 << 6,
        PepSurveyReviewed = 2 << 13,


        OccupationSurveySubmission = 2 << 7,
        OccupationSurveyDone = 2 << 8,

        SofSurveySubmission = 2 << 9,
        SofSurveyDone = 2 << 10,
        SofSurveyReviewed = 2 << 14,

        SofSubmission = 2 << 11,
        SofDone = 2 << 12,
    }
}