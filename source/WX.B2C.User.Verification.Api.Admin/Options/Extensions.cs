using Microsoft.Extensions.DependencyInjection;

namespace WX.B2C.User.Verification.Api.Admin.Options
{
    internal static class Extensions
    {
        public static IServiceCollection ConfigureHttpRedirects(this IServiceCollection services)
        {
            return services.ConfigureOptions<ConfigureHttpsRedirectionOptions>();
        }

        public static IServiceCollection ConfigureKestrelHttps(this IServiceCollection services)
        {
            return services.ConfigureOptions<ConfigureKestrelOptions>();
        }
    }
}