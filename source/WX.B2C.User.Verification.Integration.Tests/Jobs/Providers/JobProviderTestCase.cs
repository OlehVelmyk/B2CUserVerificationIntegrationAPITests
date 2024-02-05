using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Integration.Tests.Jobs.Providers
{
    public class JobProviderTestCase
    {
        private readonly Func<IResolver, Task<int>> _getTotal;
        private readonly Func<IResolver, IAsyncEnumerable<ICollection<object>>> _getObjects;
        private readonly string _readingTypeName;

        private JobProviderTestCase(Func<IResolver, Task<int>> getTotal,
                                    Func<IResolver, IAsyncEnumerable<ICollection<object>>> getObjects,
                                    string readingTypeName)
        {
            _getTotal = getTotal;
            _getObjects = getObjects;
            _readingTypeName = readingTypeName;
        }

        public static JobProviderTestCase Create<TSetting, TData>(
            Func<IResolver, IBatchJobDataProvider<TData, TSetting>> providerFactory,
            TSetting setting)
            where TSetting : BatchJobSettings where TData : IJobData
        {
            Func<IResolver, Task<int>> getTotal = resolver =>
            {
                var provider = providerFactory(resolver);
                return provider.GetTotalCountAsync(setting, CancellationToken.None);
            };

            async IAsyncEnumerable<ICollection<object>> GetObjects(IResolver resolver)
            {
                var provider = providerFactory(resolver);
                await foreach (var batch in provider.GetAsync(setting, CancellationToken.None))
                {
                    yield return batch as ICollection<object>;
                }
            }

            return new JobProviderTestCase(getTotal, GetObjects, typeof(TData).Name);
        }


        public static JobProviderTestCase Create<TSetting, TData>(
            Func<IQueryFactory, IBatchJobDataProvider<TData, TSetting>> providerFactory,
            TSetting setting)
            where TSetting : BatchJobSettings where TData : IJobData =>
            Create(resolver =>
                   {
                       var queryFactory = resolver.Resolve<IQueryFactory>();
                       return providerFactory(queryFactory);
                   },
                   setting);

        public static JobProviderTestCase Create<TSetting, TData>(
            Func<IUserVerificationQueryFactory, IBatchJobDataProvider<TData, TSetting>> providerFactory,
            TSetting setting)
            where TSetting : BatchJobSettings where TData : IJobData =>
            Create(resolver =>
                   {
                       var queryFactory = resolver.Resolve<IUserVerificationQueryFactory>();
                       return providerFactory(queryFactory);
                   },
                   setting);

        public override string ToString() =>
            $"Reading {_readingTypeName}";

        public Task<int> GetTotal(IResolver resolver) =>
            _getTotal(resolver);

        public IAsyncEnumerable<ICollection<object>> GetObjects(IResolver factory) =>
            _getObjects(factory);
    }
}