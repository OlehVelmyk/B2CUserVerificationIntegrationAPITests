using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Unit.Tests.Extensions
{
    public static class TaskRunner
    {
        /// <summary>
        /// Runs tasks in parallel.
        /// As Test runner has limited scheduler is it not enough just use collection of tasks and wait for them.
        /// It is required to call Task.Run explicitly 
        /// </summary>
        public static Func<Task> RepeatInParallel(this Func<Task> func, int count)
        {
            return async () =>
            {
                var tasks = new List<Task>();
                for (int i = 0; i < 10; i++)
                {
                    tasks.Add(Task.Run(func));
                }
                await tasks.WhenAll();
            };
        }
    }
}