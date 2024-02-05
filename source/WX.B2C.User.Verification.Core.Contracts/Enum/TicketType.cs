namespace WX.B2C.User.Verification.Core.Contracts.Enum
{
    public enum TicketType
    {
        PoaDocVerificationNeeded,
        PoiCheckResultReviewNeeded,
        SofDocVerificationNeeded,
        SofSurveyReviewNeeded,
        W9FormReviewNeeded,
        TurnoverThresholdReached,
        UkRepeatingTurnoverThresholdReached,
        AccountAlertCreated,
        UsaEddCheckListReviewNeeded,
        PepReviewNeeded,
        PassFortRiskScreeningFailed,
        LexisNexisRiskScreeningFailed,
        OnfidoKnownFacesImageQualityCheckFailed,
        AdditionalDocsReviewNeeded,
        InstantIdChecksReviewNeeded,
        PepMonthlyReviewReminder,
        ExtraHighRisk,
        NameAndDobDuplicationDetected,
        IdDocNumberDuplicationDetected,
        UsaIpMatchFailed,
        IpMatchFailed,
    }
}