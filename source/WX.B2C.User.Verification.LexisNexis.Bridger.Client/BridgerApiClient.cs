using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Xml;
using BridgerReference;

namespace WX.B2C.User.Verification.LexisNexis.Bridger.Client
{
    public class BridgerApiClient : IBridgerApiClient
    {
        private readonly BridgerCredentials _credentials;
        private readonly IBridgerClientsFactory _bridgerClientsFactory;

        public BridgerApiClient(
            Uri baseUri,
            BridgerCredentials credentials)
            : this(baseUri, credentials, new BridgerClientsFactory())
        {
        }

        public BridgerApiClient(
            Uri baseUri,
            BridgerCredentials credentials,
            IBridgerClientsFactory bridgerClientsFactory)
        {
            BaseUri = baseUri ?? throw new ArgumentNullException(nameof(baseUri));
            _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
            _bridgerClientsFactory = bridgerClientsFactory ?? throw new ArgumentNullException(nameof(bridgerClientsFactory));
        }

        public Uri BaseUri { get; }

        public string RolesOrUsers { get; set; }

        public string Reference { get; set; }

        public Task<int> GetDaysUntilPasswordExpiresAsync(string userId, string password)
        {
            var clientContext = new ClientContext
            {
                ClientID = _credentials.ClientId,
                UserID = userId,
                Password = password,
                ClientReference = Reference
            };

            var accountClient = _bridgerClientsFactory.CreateAccountClient(BaseUri, DefaultHttpsBinding);
            return accountClient.ProcessRequestAsync(async client =>
            {
                var response = await client.GetDaysUntilPasswordExpiresAsync(clientContext);
                return response.Body.GetDaysUntilPasswordExpiresResult;
            });
        }

        public Task<bool> ChangeAccountPasswordAsync(string userId, string password, string newPassword)
        {
            var clientContext = new ClientContext
            {
                ClientID = _credentials.ClientId,
                UserID = userId,
                Password = password,
                ClientReference = Reference
            };

            var accountClient = _bridgerClientsFactory.CreateAccountClient(BaseUri, DefaultHttpsBinding);
            return accountClient.ProcessRequestAsync(async client =>
            {
                var response = await client.ChangePasswordAsync(clientContext, newPassword);
                return response.Body.ChangePasswordResult;
            });
        }

        public Task<SearchResults> SearchAsync(SearchInput input, string predefinedSearchName)
        {
            var clientContext = new ClientContext
            {
                ClientID = _credentials.ClientId,
                UserID = _credentials.UserId,
                Password = _credentials.Password,
                ClientReference = Reference,
                GLB = 5,
                DPPA = DPPAChoiceType.Choice3
            };

            var config = new SearchConfiguration
            {
                PredefinedSearchName = predefinedSearchName,
                AssignResultTo = new AssignmentInfo
                {
                    RolesOrUsers = new ArrayOfString { RolesOrUsers },
                    Type = AssignmentType.Role
                },
                WriteResultsToDatabase = true
            };

            var searchClient = _bridgerClientsFactory.CreateSearchClient(BaseUri, DefaultHttpsBinding);
            return searchClient.ProcessRequestAsync(async client =>
            {
                var response = await client.SearchAsync(clientContext, config, input);
                return response.Body.SearchResult;
            });
        }

        private static BasicHttpsBinding DefaultHttpsBinding =>
            new BasicHttpsBinding
            {
                MaxReceivedMessageSize = int.MaxValue,
                MaxBufferPoolSize = int.MaxValue,
                MaxBufferSize = int.MaxValue,
                OpenTimeout = TimeSpan.FromMinutes(1),
                ReceiveTimeout = TimeSpan.FromMinutes(10),
                SendTimeout = TimeSpan.FromMinutes(10),
                ReaderQuotas = new XmlDictionaryReaderQuotas
                {
                    MaxArrayLength = int.MaxValue,
                    MaxBytesPerRead = int.MaxValue,
                    MaxDepth = int.MaxValue,
                    MaxNameTableCharCount = int.MaxValue,
                    MaxStringContentLength = int.MaxValue
                }
            };
    }
}
