using System;
using System.Threading.Tasks;

namespace WX.B2C.User.Verification.Infrastructure.Common.Monitoring
{
    internal static class TrackExtensions
    {
        public static async Task Wrap(this IDisposable track,
                                      Func<Task> taskProvider,
                                      Action<Exception> onException,
                                      Action onSuccess = null)
        {
            using (track)
            {
                var task = taskProvider();
                try
                {
                    await task;
                    onSuccess?.Invoke();
                }
                catch (Exception e)
                {
                    onException(e.GetBaseException());
                    throw;
                }
            }
        }
        
        public static async Task<T> Wrap<T>(this IDisposable track,
                                      Func<Task<T>> taskProvider,
                                      Action<Exception> onException,
                                      Action onSuccess = null)
        {
            using (track)
            {
                var task = taskProvider();
                try
                {
                    var result = await task;
                    onSuccess?.Invoke();
                    return result;
                }
                catch (Exception e)
                {
                    onException(e.GetBaseException());
                    throw;
                }
            }
        }
    }
}