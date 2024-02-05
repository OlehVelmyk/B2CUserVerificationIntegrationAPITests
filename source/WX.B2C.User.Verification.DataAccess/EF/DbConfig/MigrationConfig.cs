using System;
using System.IO;
using System.Security;
using Microsoft.Extensions.Configuration;
using WX.B2C.User.Verification.Infrastructure.Common.Configuration;
using WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults;
using WX.KeyVault;

namespace WX.B2C.User.Verification.DataAccess.EF.DbConfig
{
    internal static class MigrationConfig
    {
        private const string SettingsJsonFile = "appsettings.json";
        private const string LocalSettingsJsonFile = "appsettings.local.json";
        private const string EnvironmentSectionKey = "Environment";
        private const string LocalEnvironmentName = "local";

        internal static SecureString GetConnectionString()
        {
            var configuration = GetConfiguration();
            var env = configuration[EnvironmentSectionKey];

            if (IsLocal(env))
                return new AppLocalConfig().DbConnectionString;

            var config = GetKeyVault(configuration, env);
            return config.DbConnectionString;
        }

        internal static bool IsLocal()
        {
            var configuration = GetConfiguration();
            var env = configuration[EnvironmentSectionKey];

            return IsLocal(env);
        }

        private static bool IsLocal(string env)
        {
            return env.Equals(LocalEnvironmentName, StringComparison.OrdinalIgnoreCase);
        }

        private static IMigrationKeyVault GetKeyVault(IConfiguration configuration, string env)
        {
            var keyVaultConfig = new KeyVaultConfiguration(configuration[$"{env}:KeyVaultUrl"],
                                                           configuration[$"{env}:KeyVaultClientId"],
                                                           configuration[$"{env}:KeyVaultSecret"]);

            var keyVault = KeyVaultProxy<IMigrationKeyVault>.Create(keyVaultConfig);
            return keyVault;
        }

        private static IConfiguration GetConfiguration()
        {
            var configuration = BuildConfiguration(SettingsJsonFile);
            return configuration[EnvironmentSectionKey].StartsWith("#") ? BuildConfiguration(LocalSettingsJsonFile) : configuration;
        }

        private static IConfiguration BuildConfiguration(string path)
        {
            return new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile(path)
                   .Build();
        }
    }
}
