using System;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.Caching;

namespace WX.B2C.User.Verification.BlobStorage.Cache
{
    internal interface IMemoryCache
    {
        Task<T> GetOrCreate<T>(TimeSpan ttl, Func<Task<T>> factory);
    }

    internal class MemoryCache : IMemoryCache
    {
        private readonly IAsyncCacheProvider _cacheProvider;

        public MemoryCache(IAsyncCacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
        }

        public Task<T> GetOrCreate<T>(TimeSpan ttl, Func<Task<T>> factory)
        {
            return Policy.CacheAsync<T>(_cacheProvider, ttl)
                         .ExecuteAsync((_, _) => factory(), new Context(typeof(T).FullName), CancellationToken.None);
        }
    }
}