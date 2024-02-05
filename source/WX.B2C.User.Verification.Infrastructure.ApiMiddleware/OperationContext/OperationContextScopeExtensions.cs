using Autofac;
using Microsoft.AspNetCore.Builder;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.OperationContext
{
    public static class OperationContextScopeExtensions
    {
        public static void RegisterOperationContextScope(this ContainerBuilder builder)
        {
            builder.RegisterType<OperationContextMiddleware>().InstancePerLifetimeScope();
        }

        /// <summary>
        /// Use middleware to setup <see cref="IOperationContextProvider"/>. DO NOT FORGET to call <see cref="RegisterOperationContextScope"/>
        /// </summary>
        public static IApplicationBuilder UseOperationContextScope(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<OperationContextMiddleware>();
        }
    }
}
