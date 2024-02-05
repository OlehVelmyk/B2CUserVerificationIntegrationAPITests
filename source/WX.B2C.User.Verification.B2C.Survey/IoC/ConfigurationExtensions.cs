using System;
using Autofac;
using WX.B2C.User.Verification.B2C.Survey.Mappers;
using WX.B2C.User.Verification.Core.Contracts;

namespace WX.B2C.User.Verification.B2C.Survey.IoC
{
    public static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterB2CSurveyGateway(this ContainerBuilder builder, 
                                                                Predicate<IComponentContext> shouldUseStub)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterType<UserSurveyMapper>()
                   .As<IUserSurveyMapper>()
                   .SingleInstance();

            builder.RegisterType<B2CSurveyApiClientFactory>()
                   .As<IB2CSurveyApiClientFactory>()
                   .SingleInstance();

            builder.RegisterType<UserSurveyProvider>()
                   .As<IUserSurveyProvider>()
                   .SingleInstance();

            return builder;
        }
    }
}
