using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WX.B2C.User.Verification.Extensions
{
    public static class TaskExtensions
    {
        public static Task WhenAll(this IEnumerable<Task> tasks) =>
            Task.WhenAll(tasks);

        public static Task WhenAll(this (Task, Task) tasks) =>
            Task.WhenAll(tasks.Item1, tasks.Item2);

        public static Task<T[]> WhenAll<T>(this (Task<T>, Task<T>) tasks) =>
            Task.WhenAll(tasks.Item1, tasks.Item2);

        public static Task<T[]> WhenAll<T>(this IEnumerable<Task<T>> tasks) =>
            Task.WhenAll(tasks);

        public static async Task<(T1, T2)> WhenAll<T1, T2>(Task<T1> task1, Task<T2> task2)
        {
            await Task.WhenAll(task1, task2);
            return (task1.Result, task2.Result);
        }

        public static async Task<(T1, T2, T3)> WhenAll<T1, T2, T3>(Task<T1> task1, Task<T2> task2, Task<T3> task3)
        {
            await Task.WhenAll(task1, task2, task3);
            return (task1.Result, task2.Result, task3.Result);
        }

        public static async Task<(T1, T2, T3, T4)> WhenAll<T1, T2, T3, T4>(Task<T1> task1, Task<T2> task2, Task<T3> task3, Task<T4> task4)
        {
            await Task.WhenAll(task1, task2, task3, task4);
            return (task1.Result, task2.Result, task3.Result, task4.Result);
        }

        public static async Task<(T1, T2, T3, T4, T5)> WhenAll<T1, T2, T3, T4, T5>(Task<T1> task1, Task<T2> task2, Task<T3> task3, Task<T4> task4, Task<T5> task5)
        {
            await Task.WhenAll(task1, task2, task3, task4, task5);
            return (task1.Result, task2.Result, task3.Result, task4.Result, task5.Result);
        }

        public static async Task<(T1, T2, T3, T4, T5, T6)> WhenAll<T1, T2, T3, T4, T5, T6>(Task<T1> task1, Task<T2> task2, Task<T3> task3, Task<T4> task4, Task<T5> task5, Task<T6> task6)
        {
            await Task.WhenAll(task1, task2, task3, task4, task5, task6);
            return (task1.Result, task2.Result, task3.Result, task4.Result, task5.Result, task6.Result);
        }

        public static async Task<TResult> Select<T, TResult>(this Task<T> task, Func<T, TResult> map)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));
            if (map == null)
                throw new ArgumentNullException(nameof(map));

            var result = await task;
            return map(result);
        }

        public static void RunAndForget(this Task task, Action<Exception> handler = null)
        {
            if (task.IsCompletedSuccessfully || task.IsCanceled)
                return;

            var continuationOption = TaskContinuationOptions.ExecuteSynchronously
                                     | TaskContinuationOptions.OnlyOnFaulted;

            _ = task.ContinueWith(
                t => HandleException(t, handler),
                continuationOption);
        }

        private static void HandleException(Task task, Action<Exception> handler)
        {
            if (task.Exception == null)
                return;

            handler ??= exception => { };
            handler(task.Exception.GetBaseException());
        }
    }
}