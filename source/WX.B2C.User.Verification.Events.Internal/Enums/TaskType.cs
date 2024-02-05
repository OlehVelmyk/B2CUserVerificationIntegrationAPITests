namespace WX.B2C.User.Verification.Events.Internal.Enums
{
    public enum TaskType
    {
        // Confirm an individual is who they say they are.
        Identity = 1,
        // Confirm an individual’s address is valid and associated with that person.
        Address = 2,
        TaxResidence = 3,
        DuplicationScreening = 4,
        // Evaluate an individual’s PEPs, sanctions, and adverse media risk.
        RiskListsScreening = 5,
        FinancialCondition = 6,
        ProofOfFunds = 7,
        //NOT USED FOR NOW
        PepScreening = 8,
        FraudScreening = 9,
        EnhancedDueDiligence = 10,
        UserRiskScreening = 11,
    }
}