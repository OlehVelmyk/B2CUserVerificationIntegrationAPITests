using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Dtos
{
    public class VerificationDetailsValidationRuleDto
    {
        public string[] TaxResidences { get; set; }

        public TinValidationRuleDto[] TinValidationRules { get; set; }
    }

    public class TinValidationRuleDto
    {
        public TinType TinType { get; set; }

        public string ValidationRegex { get; set; }
    }
}