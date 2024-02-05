using System.Threading.Tasks;
using FsCheck;
using NUnit.Framework;
using WX.B2C.User.Verification.Component.Tests.Arbitraries;
using WX.B2C.User.Verification.Component.Tests.Mountebank;

namespace WX.B2C.User.Verification.Component.Tests
{
    internal abstract class BaseComponentTest
    {
        protected IOnfidoImposter OnfidoImposter;
        protected IPassfortImposter PassfortImposter;
        protected ISurveyImposter SurveyImposter;

        [OneTimeSetUp]
        protected async Task SetupContainer()
        {
            OnfidoImposter = Resolve<IOnfidoImposter>();
            PassfortImposter = Resolve<IPassfortImposter>();
            SurveyImposter = Resolve<ISurveyImposter>();

            Arb.Register<RndArbitrary>(); // TODO: WRXB-10812 Remove 
            Arb.Register<SeedArbitrary>();

            if(!OnfidoImposter.IsActive)
                await OnfidoImposter.ResetAsync();
            if(!PassfortImposter.IsActive)
                await PassfortImposter.ResetAsync();
            if (!SurveyImposter.IsActive)
                await SurveyImposter.ResetAsync();
        }

        protected T Resolve<T>() => ServiceLocator.Get<T>();
    }
}
