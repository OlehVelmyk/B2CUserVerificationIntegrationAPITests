using System.Collections.Generic;
using System.Security;

namespace WX.B2C.User.Verification.Infrastructure.Contracts.Configuration
{
    public interface IAppConfig
    {
        SecureString SplunkEndpoint { get; }

        SecureString SplunkToken { get; }

        SecureString DbConnectionString { get; }

        Dictionary<string, SecureString> EventHubNameSpaceConnectionStrings { get; }

        SecureString StorageConnectionString { get; }

        SecureString B2CStorageConnectionString { get; }

        string CommandsQueueName { get; }

        SecureString CommandsQueueConnectionString { get; }

        SecureString EventHubPrivateKey { get; }

        SecureString EventHubPublicKey { get; }

        SecureString AppInsightsInstrumentationKey { get; }

        string ServiceName { get; }

        SecureString IpStackAccessKey { get; }

        string IpStackApiUrl { get; }

        SecureString OnfidoApiToken { get; }

        string OnfidoApiUrl { get; }

        string LexisNexisRdpHost { get; }

        string LexisNexisRdpProxy { get; }

        string LexisNexisBridgerServiceEndpoint { get; }

        string LexisNexisMode { get; }

        string PassFortApiUrl { get; }

        bool CheckAutofacRegistrations { get; }

        string B2CRisksServiceUrl { get; }

        string B2CSurveyServiceUrl { get; }

        string SurveyServiceUrl { get; }

        string B2CRisksApiUrl { get; }

        string B2CSurveyApiUrl { get; }

        string SurveyApiUrl { get; }

        bool IsLocal { get; }
    }
}
