using System;
using Autofac;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Services;
using WX.B2C.User.Verification.Core.Services.RequiredData;

namespace WX.B2C.User.Verification.Api.Admin.IoC
{
    internal static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterAdminServices(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterType<ProfileProviderFactory>().As<IProfileProviderFactory>().SingleInstance();
            builder.RegisterType<NoteService>().As<INoteService>().SingleInstance();

            return builder;
        }
    }
}
