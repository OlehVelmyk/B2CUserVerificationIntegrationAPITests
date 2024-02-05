namespace WX.B2C.User.Verification.Core.Contracts.Conditions
{
    public class Condition
    {
        public ConditionType Type { get; set; }

        public object Value { get; set; }
    }

    public enum ConditionType
    {
        RiskLevel,
        TinType,
        IsPep,
        MatchCountry,
        ExceededTurnover,
        RepeatingTurnover,
        AccountAge,
        MatchDecision
    }
}