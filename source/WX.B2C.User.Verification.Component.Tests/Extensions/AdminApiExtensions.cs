using System;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;
using WX.B2C.User.Verification.Api.Admin.Client;

namespace WX.B2C.User.Verification.Component.Tests.Extensions
{
    internal static class AdminApiExtensions
    {
        public static Task<T> ExecuteUntilAsync<T>(this IUserVerificationApiClient client,
                                                   Func<IUserVerificationApiClient, Task<T>> func,
                                                   Func<T, bool> condition,
                                                   int attempts = 3)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));

            return CreatePolicy(attempts)
                .ExecuteAsync(async () =>
                {
                    var response = await func(client);
                    if (!condition(response))
                        throw new AdminApiException();

                    return response;
                });
        }

        private static AsyncRetryPolicy CreatePolicy(int retryAttempts) =>
            Policy.Handle<AdminApiException>()
                  .WaitAndRetryAsync(
                      retryAttempts,
                      retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        public class AdminApiException : Exception
        { }
    }
}
