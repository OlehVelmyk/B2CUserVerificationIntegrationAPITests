using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Polly.Caching;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.BlobStorage.Cache
{
    /// <summary>
    /// Simple cache provider. Should be used for limited number of keys because it has simple clear mechanisms.
    /// </summary>
    internal class InMemoryProvider : IAsyncCacheProvider, ISyncCacheProvider, IDisposable
    {
        private readonly ISystemClock _systemClock;
        private readonly Timer _timer;
        private readonly ConcurrentDictionary<string, (object Value, DateTime ExpiresAt)> _objects = new();

        public InMemoryProvider(TimeSpan cleanUpPeriod, ISystemClock systemClock)
        {
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
            _timer = new Timer(ClearExpired, null, TimeSpan.FromMilliseconds(-1), cleanUpPeriod);
        }

        public (bool, object) TryGet(string key)
        {
            if (!_objects.TryGetValue(key, out var tuple))
                return (false, null);

            var (value, expiresAt) = tuple;
            if (!IsExpired(expiresAt))
                return (true, value);

            return (false, null);
        }

        public void Put(string key, object value, Ttl ttl)
        {
            _objects[key] = (value, DateTime.UtcNow.Add(ttl.Timespan));
        }

        public Task<(bool, object)> TryGetAsync(string key, CancellationToken cancellationToken, bool continueOnCapturedContext)
        {
            return Task.FromResult(TryGet(key));
        }

        public Task PutAsync(string key,
                             object value,
                             Ttl ttl,
                             CancellationToken cancellationToken,
                             bool continueOnCapturedContext)
        {
            Put(key, value, ttl);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private void ClearExpired(object state)
        {
            foreach (var cachedEntity in _objects.ToArray())
            {
                if (IsExpired(cachedEntity.Value.ExpiresAt))
                    _objects.TryRemove(cachedEntity.Key, out _);
            }
        }

        private bool IsExpired(DateTime expiresAt)
        {
            return expiresAt < _systemClock.GetDate();
        }
    }
}