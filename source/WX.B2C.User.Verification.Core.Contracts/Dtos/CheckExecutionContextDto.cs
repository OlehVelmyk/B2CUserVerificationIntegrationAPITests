
namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class CheckExecutionContextDto
    {
        public CheckInputDataDto InputData { get; set; }

        public CheckExternalDataDto ExternalData { get; set; }
    }

    public class CheckExecutionResultDto
    {
        // TODO: Store serialized webhook body
    }
}
