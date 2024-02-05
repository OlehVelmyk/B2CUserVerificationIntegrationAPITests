using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Domain.Shared;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Core.Services.Utilities
{
    internal static class AppCore
    {
        public static Task<T> ApplyChangesAsync<T>(
            Func<T> getOrCreate,
            Func<T, Task> saveAndPublish) where T : AggregateRoot
        {
            return ApplyChangesAsync(getOrCreate.ToAsync(),
                                     _ => { },
                                     saveAndPublish);
        }

        public static Task<T> ApplyChangesAsync<T>(
            Func<Task<T>> getOrCreate,
            Func<T, Task> saveAndPublish) where T : AggregateRoot
        {
            return ApplyChangesAsync(getOrCreate,
                                     _ => Task.CompletedTask, 
                                     saveAndPublish);
        }

        public static Task<T> ApplyChangesAsync<T>(
            Func<Task<T>> getOrCreate,
            Action<T> update,
            Func<T, Task> saveAndPublish) where T : AggregateRoot
        {
            return ApplyChangesAsync(getOrCreate, update?.ToAsync(), saveAndPublish);
        }

        public static Task<T> ApplyChangesAsync<T>(
            Func<Task<T>> getOrCreate,
            Func<T, Task> update,
            Func<T, Task> saveAndPublish) where T : AggregateRoot
        {
            if (getOrCreate == null)
                throw new ArgumentNullException(nameof(getOrCreate));
            if (update == null)
                throw new ArgumentNullException(nameof(update));
            if (saveAndPublish == null)
                throw new ArgumentNullException(nameof(saveAndPublish));

            return ApplyChanges(getOrCreate, update, saveAndPublish);
        }

        private static async Task<T> ApplyChanges<T>(
            Func<Task<T>> getOrCreate,
            Func<T, Task> update,
            Func<T, Task> saveAndPublish) where T : AggregateRoot
        {
            var aggregate = await getOrCreate.Invoke();
            if (aggregate == null)
                throw new InvalidOperationException("Aggregate root could not be null.");

            await update.Invoke(aggregate);

            if (aggregate.HasChanges)
                await saveAndPublish(aggregate);

            return aggregate;
        }
    }
}