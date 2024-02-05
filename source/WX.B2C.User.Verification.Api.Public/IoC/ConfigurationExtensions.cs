using System;
using Autofac;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Providers;
using WX.B2C.User.Verification.Core.Contracts.Validation;
using WX.B2C.User.Verification.Core.Services;
using WX.B2C.User.Verification.Core.Services.Providers;
using WX.B2C.User.Verification.Core.Services.RequiredData;
using WX.B2C.User.Verification.Core.Services.Validation;

namespace WX.B2C.User.Verification.Api.Public.IoC
{
    internal static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterPublicServices(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterType<ActionService>().As<IActionService>().SingleInstance();
            builder.RegisterType<RegionActionsProvider>().As<IRegionActionsProvider>().SingleInstance();
            builder.RegisterType<UserActionFactory>().As<IUserActionFactory>().SingleInstance();
            builder.RegisterType<PolicySelectionContextProvider>().As<IPolicySelectionContextProvider>().SingleInstance();
            builder.RegisterType<ProfileProviderFactory>().As<IProfileProviderFactory>().SingleInstance();
            builder.RegisterType<TokenService>().As<ITokenService>().SingleInstance();
            builder.RegisterType<ValidationRuleProvider>().As<IValidationRuleProvider>().SingleInstance();
            builder.RegisterType<FundsDocumentValidationRuleFilter>().As<IValidationRuleFilter>().SingleInstance();

            return builder;
        }
    }
}
