using System;
using System.Collections.Generic;
using System.IO;
using Autofac;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Serilog;
using Serilog.Events;
using WX.B2C.User.Verification.Core.Services.IoC;
using WX.B2C.User.Verification.DataAccess.IoC;
using WX.B2C.User.Verification.Infrastructure.Common.Configuration;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.B2C.User.Verification.Integration.Tests.Fixtures;
using WX.Core.TypeExtensions;
using WX.KeyVault;
using WX.Logging;
using WX.Logging.Autofac;

namespace WX.B2C.User.Verification.Integration.Tests
{
    using static Constants;

    public abstract class BaseIntegrationTest
    {
        private IContainer _container;

        internal DbFixture DbFixture => Resolve<DbFixture>();

        [OneTimeSetUp]
        public void SetupContainer()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<AppLocalConfig>().As<IAppConfig>().SingleInstance();
            RegisterSettingConfiguration(containerBuilder);
            containerBuilder.RegisterLogging((_, configuration) => configuration.WithMinimumLogLevel(LogEventLevel.Debug).WriteToDebug().WriteTo.Console());
            containerBuilder.RegisterDataAccess();
            containerBuilder.RegisterCommonServices();
            containerBuilder.Register(context => new DbFixture(context.Resolve<IAppConfig>().DbConnectionString.UnSecure())).SingleInstance();
            RegisterModules(containerBuilder);
            _container = containerBuilder.Build();
        }

        protected virtual void RegisterModules(ContainerBuilder containerBuilder)
        {   }

        protected void RegisterOptions<T>(ContainerBuilder containerBuilder, string pathToSetting, string key)
            where T : class
        {
            if (!File.Exists(pathToSetting))
                throw new FileNotFoundException("File was not found.", pathToSetting);
            
            var json = File.ReadAllText(pathToSetting);
            var options = JObject.Parse(json).GetValue(key)?.ToObject<T>() 
                          ?? throw new KeyNotFoundException($"Key: {key} was not found in config.");
            containerBuilder.RegisterInstance(options).SingleInstance();
        }

        protected string GetPath(params string[] paths)
        {
            var path = Path.Combine(paths);

            if (!File.Exists(path) && !Directory.Exists(path))
                throw new InvalidOperationException($"Path {path} was not found.");

            return path;
        }

        protected T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        protected static void RegisterKeyVault<TKeyVault>(ContainerBuilder containerBuilder) 
        {
            containerBuilder.Register(context =>
            {
                var configuration = context.Resolve<IConfiguration>();
                var keyVaultConfiguration = new KeyVaultConfiguration(
                    configuration[KeyVault.UrlPath],
                    configuration[KeyVault.ClientIdPath],
                    configuration[KeyVault.SecretPath]);
                return KeyVaultProxy<TKeyVault>.Create(keyVaultConfiguration);
            }).As<TKeyVault>().SingleInstance();
        }

        private static void RegisterSettingConfiguration(ContainerBuilder containerBuilder)
        {
            var configuration = new ConfigurationBuilder()
                                .SetBasePath(RootPath)
                                .AddJsonFile(SettingsFilePath)
                                .Build();

            containerBuilder.RegisterInstance(configuration)
                            .As<IConfiguration>()
                            .SingleInstance();
        }
    }
}