using WX.B2C.User.Verification.Provider.Contracts;

namespace WX.B2C.User.Verification.Provider.Services.System
{
    internal sealed class TaxResidenceCheckProviderFactory : BaseCheckProviderFactory<TaxResidenceCheckConfiguration>
    {
        protected override CheckProvider Create(TaxResidenceCheckConfiguration configuration)
        {
            var checkDataValidator = new TaxResidenceCheckDataValidator(configuration);
            var checkRunner = new TaxResidenceCheckRunner(configuration.Countries);
            return CheckProvider.Create(checkDataValidator, checkRunner);
        }
    }
}