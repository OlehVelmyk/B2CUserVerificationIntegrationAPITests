using System;
using Autofac;
using WX.B2C.User.Verification.Facade.Controllers.Internal.Mappers;

namespace WX.B2C.User.Verification.Facade.Controllers.Internal.IoC
{
    public static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterInternalFacade(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterMappers();

            return builder;
        }

        private static void RegisterMappers(this ContainerBuilder builder)
        {
            builder.RegisterType<DocumentMapper>()
                   .As<IDocumentMapper>()
                   .SingleInstance();

            builder.RegisterType<ContentTypeMapper>()
                   .As<IContentTypeMapper>()
                   .SingleInstance();

            builder.RegisterType<VerificationDetailsMapper>()
                   .As<IVerificationDetailsMapper>()
                   .SingleInstance();

            builder.RegisterType<ExternalProfileMapper>()
                   .As<IExternalProfileMapper>()
                   .SingleInstance();

            builder.RegisterType<ApplicationMapper>()
                   .As<IApplicationMapper>()
                   .SingleInstance();

            builder.RegisterType<CountryMapper>()
                   .As<ICountryMapper>()
                   .SingleInstance();

            builder.RegisterType<SdkTokenMapper>()
                   .As<ISdkTokenMapper>()
                   .SingleInstance();
        }
    }
}