using System;
using System.Collections.Generic;
using System.Security;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.Core.TypeExtensions;

namespace WX.B2C.User.Verification.Infrastructure.Common.Configuration
{
    public class AppLocalConfig : IAppConfig
    {
        public SecureString SplunkEndpoint => "http://localhost:8088/services/collector".Secure();

        public SecureString SplunkToken => "ae8bb302-1bb4-4022-a7b4-d2e1c97df539".Secure();
        
        //Dev3
        //public SecureString DbConnectionString => "Server=tcp:wx-fabric-dev.database.windows.net,1433;Initial Catalog=wx-fabric-b2c-user-verification-dev-3-db;Persist Security Info=False;User ID=wx-fabric-b2c-user-verification-dev-3-db-admin;Password=s4GukHiE5zeh23a0;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;".Secure();
        
        public SecureString DbConnectionString => "Server=(localdb)\\.\\sharedb;Database=wx-b2c-user-verification-local;Trusted_Connection=True;MultipleActiveResultSets=true;Integrated Security=True;".Secure();

        public Dictionary<string, SecureString> EventHubNameSpaceConnectionStrings =>
            new Dictionary<string, SecureString>
            {

            };

        public SecureString StorageConnectionString => "DefaultEndpointsProtocol=https;AccountName=wxdev3stga1;AccountKey=S3ymkvnjN77tppbq0+YYkxAH2FeD6Q7d54ageUaEppoLsK01oJ4yFP7RAOW1b28TcqfqEDu1IOFQ9TPw1N+h3Q==;EndpointSuffix=core.windows.net".Secure();

        public SecureString B2CStorageConnectionString => "DefaultEndpointsProtocol=https;AccountName=wxdev3stga1;AccountKey=S3ymkvnjN77tppbq0+YYkxAH2FeD6Q7d54ageUaEppoLsK01oJ4yFP7RAOW1b28TcqfqEDu1IOFQ9TPw1N+h3Q==;EndpointSuffix=core.windows.net".Secure();

        public string CommandsQueueName => CommandQueueNameResolver.Get();

        public SecureString CommandsQueueConnectionString => "DefaultEndpointsProtocol=https;AccountName=wxlocal1stga1;AccountKey=PvUUGY4TDwItPWb8mRLor62BjWK4BGqi9njOO8V9MjpFvGPswLClCV5E7qVOtUnATMvl/7LNBL244RXB5CO+/Q==;EndpointSuffix=core.windows.net"
                .Secure();

        public SecureString EventHubPrivateKey => "".Secure();

        public SecureString EventHubPublicKey => "".Secure();

        public SecureString AppInsightsInstrumentationKey => "".Secure();

        public SecureString ApplicationConfigurationConnectionString =>
            "Endpoint=https://wx-user-dev3-appc.azconfig.io;Id=EUH3-l9-s0:mawNsS44l4X5bcYqHzjB;Secret=x9l20gZJ5DMUty2hAI/2qCrkFkydM3yiYUkC+7d1jZ0=".Secure();

        public SecureString ConfigurationServiceBusConnectionString =>
            "Endpoint=sb://wx-user-dev3-sbn.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=vAWmQ5YjQN2IRBAe4yVCl7sdMaW/2vgrGwKF2DsruSM="
                .Secure();

        public string ServiceName => "wx-b2c-userverification-dev";

        public SecureString IpStackAccessKey => "3ccd866ee121a006861cf3f4ee8c61ce".Secure();

        public string IpStackApiUrl => "https://api.ipstack.com";

        public SecureString OnfidoApiToken => "Token token=test_b1ffs76SNj8WnsqCOBIarHNUBc9GNtD9".Secure();

        public string OnfidoApiUrl => UseImposter ? "http://localhost:0001/v3.2" : "https://api.eu.onfido.com/v3.2";

        public string LexisNexisRdpHost => "https://staging.ws.idms.lexisnexis.com";

        public string LexisNexisRdpProxy => "http://13.83.16.248:3128/";

        public string LexisNexisBridgerServiceEndpoint => "https://bridgerstaging.lexisnexis.com/LN.WebServices/11.2/XGServices.svc/";

        public string LexisNexisMode => "testing";

        public string PassFortApiUrl => UseImposter ? "http://localhost:0002/4.0" : "https://api.passfort.com/4.0";

        public bool CheckAutofacRegistrations => true;

        public string SurveyApiUrl => "http://localhost:17062";

        public string B2CRisksServiceUrl => "fabric:/WX.B2C.Risks/ApiService";

        public string B2CSurveyServiceUrl => "fabric:/WX.B2C.Survey/InternalApi";

        public string SurveyServiceUrl => "fabric:/WX.Survey/WX.Survey.Api";

        public string B2CRisksApiUrl => "http://localhost:17064";

        public string B2CSurveyApiUrl => "http://localhost:17063";

        public bool IsLocal => true;

        private bool UseImposter => true;
    }
}
