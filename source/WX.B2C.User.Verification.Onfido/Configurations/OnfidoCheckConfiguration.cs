using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Onfido.Configurations
{
    internal abstract class OnfidoCheckConfiguration : CheckProviderConfiguration
    {
        public string ReportName { get; set; }
    }
}
