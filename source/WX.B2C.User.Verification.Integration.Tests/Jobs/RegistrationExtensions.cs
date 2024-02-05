using Autofac;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess;

namespace WX.B2C.User.Verification.Integration.Tests.Jobs
{
    public static class RegistrationExtensions
    {
        public static ContainerBuilder RegisterDbQueryFactory(this ContainerBuilder builder)
        {
            builder.RegisterType<B2CUserVerificationQueryFactory>().As<IQueryFactory>().SingleInstance();

            return builder;
        }
    }
}