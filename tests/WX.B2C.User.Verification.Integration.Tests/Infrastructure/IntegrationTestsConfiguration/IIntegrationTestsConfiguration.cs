using WX.Configuration.Admin.Interface;
using WX.Configuration.Contracts.Interface;
using WX.Configuration.Contracts.Tests.Interface;
using WX.Configuration.Regions.Interface;
using WX.Configuration.ServiceDiscovery.Interface;

namespace WX.B2C.User.Verification.Integration.Tests.Infrastructure.IntegrationTestsConfiguration;

public interface IIntegrationTestsConfiguration : ILoggerConfiguration,
                                                  IApplicationInformation,
                                                  IHostEnvironmentInformation,
                                                  IAdminConfiguration,
                                                  IKeyVaultConfiguration,
                                                  IEndpointsInformation,
                                                  ISnapshotsInformation,
                                                  IServiceEndpointsInformation,
                                                  IAdminLoginInformation,
                                                  IRegionsConfiguration
{
}