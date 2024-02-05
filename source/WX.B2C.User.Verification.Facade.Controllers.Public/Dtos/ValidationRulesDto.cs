namespace WX.B2C.User.Verification.Facade.Controllers.Public.Dtos
{
    public class ValidationRulesDto
    {
        public VerificationDetailsValidationRuleDto VerificationDetailsRule { get; set; }

        public DocumentValidationRuleDto[] DocumentRules { get; set; }
    }
}
