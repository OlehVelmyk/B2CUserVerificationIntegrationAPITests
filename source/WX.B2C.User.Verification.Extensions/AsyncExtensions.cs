using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WX.B2C.User.Verification.Extensions
{
    public static class AsyncExtensions
    {
        public static Func<T, Task> ToAsync<T>(this Action<T> action)
        {
            return value =>
            {
                action?.Invoke(value);
                return Task.CompletedTask;
            };
        }

        public static Func<Task<T>> ToAsync<T>(this Func<T> action)
        {
            return () =>
            {
                if (action == null)
                    return Task.FromResult<T>(default);

                var result = action.Invoke();
                return Task.FromResult(result);
            };
        }
    }
}
