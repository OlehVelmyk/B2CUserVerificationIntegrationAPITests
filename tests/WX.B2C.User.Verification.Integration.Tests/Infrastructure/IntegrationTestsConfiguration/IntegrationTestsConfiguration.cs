using WX.Configuration.Admin.Data;
using WX.Configuration.Contracts.Data;
using WX.Configuration.Contracts.Snapshots;
using WX.Configuration.Contracts.Tests.Data;
using WX.Configuration.Regions.Contracts;
using WX.Configuration.ServiceDiscovery.Data;

namespace WX.B2C.User.Verification.Integration.Tests.Infrastructure.IntegrationTestsConfiguration;

public class IntegrationTestsConfiguration : IIntegrationTestsConfiguration
{
    public EnvironmentData Environment { get; set; }
    public ApplicationData Application { get; set; }
    public EndpointData[] Endpoints { get; set; }
    public LoggerData Logger { get; set; }
    public AdminConfigurationData Admin { get; set; }
    public KeyVaultData KeyVaultConfigSection { get; set; }
    public SnapshotInfo[] AllSnapshots { get; set; }
    public SnapshotInfo[] CurrentSnapshots { get; set; }
    public ServiceEndpointDictionary<ServiceEndpointData> ServiceEndpoints { get; set; }
    public AdminLoginData AdminLoginData { get; set; }
    public CaseInsensitiveStringDictionary<RegionDefinition> Regions { get; set; }
}