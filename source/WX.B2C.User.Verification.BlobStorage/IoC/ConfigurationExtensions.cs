using System;
using Autofac;
using Polly.Caching;
using WX.B2C.User.Verification.BlobStorage.Cache;
using WX.B2C.User.Verification.BlobStorage.Configurations;
using WX.B2C.User.Verification.BlobStorage.Dto;
using WX.B2C.User.Verification.BlobStorage.Factories;
using WX.B2C.User.Verification.BlobStorage.Mappers;
using WX.B2C.User.Verification.BlobStorage.Providers;
using WX.B2C.User.Verification.BlobStorage.Storages;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;
using WX.B2C.User.Verification.Domain.Shared;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.Core.TypeExtensions;

namespace WX.B2C.User.Verification.BlobStorage.IoC
{
    public static class ConfigurationExtensions
    {
        public static ContainerBuilder RegisterBlobStorage(this ContainerBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.RegisterFactories()
                          .RegisterStorages()
                          .RegisterOptions()
                          .RegisterCache()
                          .RegisterMappers()
                          .RegisterJsonConfigurations();
        }

        private static ContainerBuilder RegisterJsonConfigurations(this ContainerBuilder builder)
        {
            builder.AddBlobJson<CountriesDto>("b2c/configuration/country-states.json");
            builder.AddBlobJson<PhoneCodeDto[]>("b2c/configuration/phone-codes.json");
            builder.AddBlobJson<ExcludedNameDto[]>("b2c/configuration/excluded-names.json");

            return builder;
        }

        private static ContainerBuilder RegisterFactories(this ContainerBuilder builder)
        {
            builder.RegisterType<BlobContainerClientFactory>().As<IBlobContainerClientFactory>().SingleInstance();

            return builder;
        }

        private static ContainerBuilder RegisterOptions(this ContainerBuilder builder)
        {
            builder.RegisterType<OptionsProvider>()
                   .As<IOptionProvider>()
                   //For using option provider explicitly
                   .As<IOptionProvider<CountriesOption>>()
                   .As<IOptionProvider<PhoneCodesOption>>()
                   .As<IOptionProvider<ExcludedNamesOption>>()
                   .SingleInstance();

            return builder;
        }

        private static ContainerBuilder RegisterStorages(this ContainerBuilder builder)
        {
            builder.RegisterType<Storages.BlobStorage>().As<IBlobStorage>().SingleInstance();
            builder.RegisterType<FileBlobStorage>().As<IFileBlobStorage>().SingleInstance();
            builder.RegisterType<ConfigurationBlobStorage>().As<IConfigurationBlobStorage>().SingleInstance();
            builder.RegisterType<CsvBlobStorage>().As<ICsvBlobStorage>().SingleInstance();
            builder.RegisterType<ReportsBlobStorage>().As<IReportsBlobStorage>().SingleInstance();

            return builder;
        }

        private static ContainerBuilder RegisterCache(this ContainerBuilder builder)
        {
            builder.RegisterType<MemoryCache>().As<IMemoryCache>().SingleInstance();
            builder.Register(context =>
                            {
                                var clock = context.Resolve<ISystemClock>();
                                return new InMemoryProvider(TimeSpan.FromDays(1), clock);
                            })
                            .As<IAsyncCacheProvider>()
                            .SingleInstance();

            return builder;
        }

        private static ContainerBuilder RegisterMappers(this ContainerBuilder builder)
        {
            builder.RegisterType<OptionMapper>().As<IOptionMapper>().SingleInstance();
            return builder;
        }

        private static ContainerBuilder AddBlobJson<T>(this ContainerBuilder builder, string path)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (path == null) throw new ArgumentNullException(nameof(path));

            if (path.IndexOf('/') == -1)
                throw new ArgumentException("Path should consist of container and blob name", nameof(path));

            builder.Register(context =>
            {
                var appConfig = context.Resolve<IAppConfig>();
                return new BlobJsonConfiguration
                {
                    ConnectionString = appConfig.B2CStorageConnectionString.UnSecure(),
                    ContainerName = path.Substring(0, path.IndexOf('/')),
                    BlobPath = path.Substring(path.IndexOf('/') + 1),
                    ReloadOnChange = true,
                };
            }).Keyed<BlobJsonConfiguration>(typeof(T));

            return builder;
        }
    }
}