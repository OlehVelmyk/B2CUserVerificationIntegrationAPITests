using System;
using System.Collections.Specialized;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.Core.TypeExtensions;

namespace WX.B2C.User.Verification.Worker.Jobs.Quartz
{
    public interface IQuartzPropertiesProvider
    {
        NameValueCollection GetProperties();
    }

    internal class QuartzPropertiesProvider : IQuartzPropertiesProvider
    {
        private readonly IAppConfig _appConfig;

        public QuartzPropertiesProvider(IAppConfig appConfig)
        {
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
        }

        public NameValueCollection GetProperties()
        {
            return new NameValueCollection
            {
                { "quartz.jobStore.misfireThreshold", "60000" },
                { "quartz.jobStore.type", "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz" },
                { "quartz.jobStore.useProperties", "true" },
                { "quartz.jobStore.dataSource", "default" },
                { "quartz.jobStore.tablePrefix", "[jobs].QRTZ_" },
                { "quartz.jobStore.clustered", "true" },
                { "quartz.jobStore.clusterCheckinInterval", "20000" },
                { "quartz.jobStore.driverDelegateType", "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz" },
                { "quartz.dataSource.default.connectionString", _appConfig.DbConnectionString.UnSecure() },
                { "quartz.dataSource.default.provider", "SqlServer" },
                { "quartz.serializer.type", "json" },
                { "quartz.scheduler.instanceId", "AUTO" }
            };
        }
    }
}
