using System.Runtime.CompilerServices;
using Polly;
using WX.B2C.User.Verification.Integration.Tests.Constants;

namespace WX.B2C.User.Verification.Integration.Tests.Extensions;

internal static class ApiExtensions
{
    public static Task<TResult> ExecuteUntilAsync<TResult, TClient>(this TClient client, 
                                                                    Func<TClient, Task<TResult>> func, 
                                                                    Func<TResult, bool> condition, 
                                                                    int attempts = Timeouts.DefaultRetryAttempts,
                                                                    [CallerArgumentExpression("func")] string? resourceExpression = null,
                                                                    [CallerArgumentExpression("condition")] string? conditionExpression = null)
    {
        if (client == null)
            throw new ArgumentNullException(nameof(client));
        if (func == null)
            throw new ArgumentNullException(nameof(func));
        if (condition == null)
            throw new ArgumentNullException(nameof(condition));

        return Policy<TResult>.HandleResult(result => !condition(result))
                        .WaitAndRetryAsync(
                            attempts,
                            Timeouts.GetTimeout,
                            (_, waitTime, retryAttempt, _) => 
                                Console.WriteLine($"Start retry due to condition {conditionExpression} not passed for resource {resourceExpression}. Retry attempt: {retryAttempt}, timeout: {waitTime}"))
                        .ExecuteAsync(async () => await func(client));
    }
}
