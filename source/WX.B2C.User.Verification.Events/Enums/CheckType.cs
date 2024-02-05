namespace WX.B2C.User.Verification.Events.Enums
{
    public enum CheckType
    {
        IdentityDocument = 1,
        IdentityEnhanced = 2,
        FacialSimilarity = 3,
        TaxResidence = 4,
        IpMatch = 5,
        FaceDuplication = 6,
        NameAndDoBDuplication = 7,
        IdDocNumberDuplication = 8,
        FraudScreening = 9,
        RiskListsScreening = 10,
        Address = 11,
        SurveyAnswers = 12
    }
}
