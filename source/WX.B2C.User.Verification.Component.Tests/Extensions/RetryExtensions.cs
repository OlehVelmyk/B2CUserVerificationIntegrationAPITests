using System;
using System.Threading.Tasks;
using Polly;

namespace WX.B2C.User.Verification.Component.Tests.Extensions
{
    public static class RetryExtensions
    {
        private const int MaxRetryCount = 10;

        public static async Task<T> WaitUntil<T>(this Func<Task<T>> act, Predicate<T> condition)
        {
            var policy =
                Policy.HandleResult<T>(result => !condition(result))
                      .WaitAndRetryAsync(MaxRetryCount,
                                         i =>
                                         {
                                             Console.WriteLine($"{DateTime.UtcNow:HH:mm:ss.ffff}:{i} iteration failed");
                                             return TimeSpan.FromMilliseconds(200 * i);
                                         });

            var result = await policy.ExecuteAsync(act);

            return result;
        }
    }
}