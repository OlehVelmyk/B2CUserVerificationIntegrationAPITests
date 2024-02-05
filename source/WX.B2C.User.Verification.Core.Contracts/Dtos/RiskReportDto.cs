using WX.B2C.User.Verification.Domain.Profile;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class RiskReportDto
    {
        public RiskLevel RiskLevel { get; set; }

        public string[] EvaluatedRiskFactors { get; set; }
    }
}
