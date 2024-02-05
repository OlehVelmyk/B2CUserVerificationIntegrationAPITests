using System;
using Autofac;
using Microsoft.AspNetCore.Http;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Facade.Controllers.Public.Mappers;
using WX.B2C.User.Verification.Facade.Controllers.Public.Providers;
using WX.B2C.User.Verification.Facade.Controllers.Public.Requests;
using WX.B2C.User.Verification.Facade.Controllers.Public.Services;
using WX.B2C.User.Verification.Facade.Controllers.Public.Validators;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Validation;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.IoC
{
    public static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterPublicFacade(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder
                   .RegisterServices()
                   .RegisterMappers()
                   .RegisterAsyncValidators();
        }

        private static ContainerBuilder RegisterServices(this ContainerBuilder builder)
        {
            builder.RegisterType<ProfileAggregationService>()
                   .As<IProfileAggregationService>()
                   .SingleInstance();

            builder.RegisterType<UserActionValidatorService>()
                    .As<IUserActionValidatorService>()
                    .SingleInstance();

            builder.RegisterType<HttpContextAccessor>()
                   .As<IHttpContextAccessor>()
                   .SingleInstance();

            builder.RegisterType<VerificationPolicyProvider>()
                   .As<IVerificationPolicyProvider>()
                   .SingleInstance();

            builder.RegisterDecorator<IVerificationPolicyProvider>((context, _, provider) =>
            {
                var profileService = context.Resolve<IProfileService>();
                var profileStorage = context.Resolve<IProfileStorage>();
                var httpContextAccessor = context.Resolve<IHttpContextAccessor>();
                var hostSettingsProvider = context.Resolve<IHostSettingsProvider>();

                var environment = hostSettingsProvider.GetSetting(HostSettingsKey.Environment);
                var isProduction = environment.Equals("Production", StringComparison.InvariantCultureIgnoreCase);

                return isProduction ? provider : new VerificationPolicyProviderDecorator(provider, profileService, profileStorage, httpContextAccessor);
            });

            return builder;
        }

        private static ContainerBuilder RegisterMappers(this ContainerBuilder builder)
        {
            builder.RegisterType<ActionMapper>()
                   .As<IActionMapper>()
                   .SingleInstance();

            builder.RegisterType<FileMapper>()
                   .As<IFileMapper>()
                   .SingleInstance();

            builder.RegisterType<DocumentMapper>()
                   .As<IDocumentMapper>()
                   .SingleInstance();

            builder.RegisterType<SdkTokenMapper>()
                   .As<ISdkTokenMapper>()
                   .SingleInstance();

            builder.RegisterType<ValidationRulesMapper>()
                   .As<IValidationRulesMapper>()
                   .SingleInstance();

            builder.RegisterType<VerificationDetailsMapper>()
                   .As<IVerificationDetailsMapper>()
                   .SingleInstance();

            builder.RegisterType<ActionTypeMapper>()
                   .As<IActionTypeMapper>()
                   .SingleInstance();

            builder.RegisterType<DocumentCategoryMapper>()
                .As<IDocumentCategoryMapper>()
                .SingleInstance();

            return builder;
        }

        private static ContainerBuilder RegisterAsyncValidators(this ContainerBuilder builder)
        {
            builder.RegisterType<SubmitDocumentRequestValidator>()
                .As<RequestAsyncValidator<SubmitDocumentRequest>>()
                .SingleInstance();

            builder.RegisterType<UpdateVerificationDetailsRequestValidator>()
                .As<RequestAsyncValidator<UpdateVerificationDetailsRequest>>()
                .SingleInstance();

            builder.RegisterType<UploadDocumentFileRequestValidator>()
                .As<RequestAsyncValidator<UploadDocumentFileRequest>>()
                .SingleInstance();

            return builder;
        }
    }
}
