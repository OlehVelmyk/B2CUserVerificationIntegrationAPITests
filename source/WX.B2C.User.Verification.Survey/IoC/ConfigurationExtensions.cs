using System;
using Autofac;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Survey.Mappers;

namespace WX.B2C.User.Verification.Survey.IoC
{
    public static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterSurveyGateway(this ContainerBuilder builder, 
                                                             Predicate<IComponentContext> shouldUseStub)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterType<UserSurveyTemplateMapper>()
                   .As<IUserSurveyTemplateMapper>()
                   .SingleInstance();

            builder.RegisterType<SurveyApiClientFactory>()
                   .As<ISurveyApiClientFactory>()
                   .SingleInstance();

            builder.RegisterType<SurveyTemplatesProvider>().AsSelf().SingleInstance();
            builder.RegisterType<SurveyTemplatesProviderStub>().AsSelf().SingleInstance();

            builder.Register<ISurveyTemplatesProvider>(context => shouldUseStub(context)
                       ? context.Resolve<SurveyTemplatesProviderStub>()
                       : context.Resolve<SurveyTemplatesProvider>())
                   .As<ISurveyTemplatesProvider>()
                   .SingleInstance();

            return builder;
        }
    }
}
