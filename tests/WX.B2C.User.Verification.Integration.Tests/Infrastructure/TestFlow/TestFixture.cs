using System.Net;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Polly;
using RestEase.HttpClientFactory;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using WX.B2C.User.Profile.Api.Client.Public;
using WX.B2C.User.Profile.Api.Client.Public.Contracts;
using WX.B2C.User.Verification.Integration.Tests.Api;
using WX.B2C.User.Verification.Integration.Tests.Constants;
using WX.B2C.User.Verification.Integration.Tests.Core.ApiClients;
using WX.B2C.User.Verification.Integration.Tests.Core.DelegatingHandlers;
using WX.B2C.User.Verification.Integration.Tests.Factories;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure.EventsService;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure.FabricAddressResolver;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure.IntegrationTestsConfiguration;
using WX.B2C.User.Verification.Integration.Tests.Infrastructure.TestFlow;
using WX.B2C.User.Verification.Integration.Tests.Providers;
using WX.B2C.User.Verification.Integration.Tests.Steps;
using WX.B2C.User.Verification.Integration.Tests.Steps.AdditionalConditions;
using WX.B2C.User.Verification.Integration.Tests.Steps.Admin;
using WX.B2C.User.Verification.Integration.Tests.Steps.ApplicationActions;
using WX.Configuration.Admin;
using WX.Configuration.Contracts.Options;
using WX.Configuration.Contracts.Tests;
using WX.Configuration.IoC;
using WX.Configuration.KeyVault;
using WX.Configuration.Regions;
using WX.Configuration.SelfHost;
using WX.Configuration.ServiceDiscovery;
using WX.Integration.Tests.Api.Client.Internal;
using WX.Integration.Tests.Api.Client.Internal.Contracts;
using WX.Integration.Tests.EventListener.Common;
using WX.Integration.Tests.EventListener.EventListener;
using WX.Integration.Tests.EventListener.IoC;
using WX.KeyVault;
using WX.Preconditions;
using WX.Preconditions.Contracts.Attributes;
using IPublicClient = WX.B2C.User.Verification.Api.Public.Client.IUserVerificationApiClient;
using ISurveyPublicClient = WX.B2C.Survey.Api.Public.Client.IB2CSurveyApiClient;
using AdminApi = WX.B2C.User.Verification.Api.Admin.Client.Models;

namespace WX.B2C.User.Verification.Integration.Tests;

[SetUpFixture]
internal class TestFixture
{
    private const string RisksAdminApiFabricUrl = "fabric:/WX.B2C.Risks/ApiAdminService";
    private const string ProfileAdminApiFabricUrl = "fabric:/WX.B2C.User.Profile/AdminApi";
    private const string VerificationAdminApiFabricUrl = "fabric:/WX.B2C.User.Verification/AdminApi";
    private const string IntegrationTestsInternalApiFabricUrl = "fabric:/WX.Integration.Tests/InternalApi";
    private static Settings _settings;
    private static IConfiguration _configuration;
    private BroadcastListener _broadcastListener;
    private CancellationTokenSource _cancellationTokenSource;
    private EventSink _eventSink;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _eventSink = ServiceLocator.Provider.GetRequiredService<EventSink>();
        _broadcastListener = ServiceLocator.Provider.GetRequiredService<BroadcastListener>();
        _cancellationTokenSource = new CancellationTokenSource();
        _eventSink.Start(_cancellationTokenSource.Token);
        _broadcastListener.Start(x => _eventSink.Post(x), _cancellationTokenSource.Token);

        AssertionOptions.FormattingOptions.MaxLines = 1000;
    }

    [OneTimeTearDown]
    public void OneTimeTeardown()
    {
        _cancellationTokenSource.Cancel();
    }

    [DependencyInjectionExtension]
    public static async Task RegisterServices()
    {
        RegisterSettingConfiguration(ServiceLocator.Services);
        RegisterDelegatingHandlers(ServiceLocator.Services);
        ConfigureServices(ServiceLocator.Services);
        RegisterApiClients(ServiceLocator.Services);
        RegisterConfiguration(ServiceLocator.Services);
        RegisterSteps(ServiceLocator.Services);
        RegisterPublishers(ServiceLocator.Services);
    }

    private static void RegisterSteps(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<TurnoverProvider>();
        serviceCollection.AddTransient<SignInStep>();
        serviceCollection.AddTransient<RegisterUserStep>();
        serviceCollection.AddScoped<TopUpTurnoverStepFactory>();

        RegisterUserSteps(serviceCollection);
        RegisterAdminSteps(serviceCollection);
        RegisterStepAdditionalConditions(serviceCollection);
    }

    private static void RegisterUserSteps(IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<CreateApplicationStep>();
        serviceCollection.AddTransient<SubmitTaxResidenceStep>();
        serviceCollection.AddTransient<SubmitDocumentStepFactory>();
        serviceCollection.AddTransient<SubmitTinStep>();
        serviceCollection.AddTransient<SubmitSurveyStepFactory>();
    }

    private static void RegisterAdminSteps(IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<AdminReviewSurveyStepFactory>();
        serviceCollection.AddTransient<AdminUpdateVerificationDetailsStepFactory>();
        serviceCollection.AddTransient<AdminUpdateUserFullNameStepFactory>();
        serviceCollection.AddTransient<AdminRequestSurveyStepFactory>();
        serviceCollection.AddTransient<AdminApproveTaskStepFactory>();
        serviceCollection.AddTransient<AdminRequestDocumentStepFactory>();
        serviceCollection.AddTransient<AdminReviewDocumentStepFactory>();
        serviceCollection.AddTransient<AdminRejectApplicationStep>();
        serviceCollection.AddTransient<AdminRevertApplicationDecisionStepFactory>();
        serviceCollection.AddSingleton<AdminSetRiskLevelStepFactory>();
    }

    private static void RegisterStepAdditionalConditions(IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<ApplicationStateConditionFactory>();
        serviceCollection.AddTransient<ActionOpenConditionFactory>();
        serviceCollection.AddTransient<AvailableActionsCountStepFactory>();
        serviceCollection.AddTransient<CollectionStepsContainsStepCondtionFactory>();
        serviceCollection.AddTransient<ChecksConditionFactory>();
        serviceCollection.AddTransient<TaskAssignedConditionFactory>();
    }

    private static void RegisterPublishers(IServiceCollection builder)
    {
        builder.RegisterListenerServices();
        builder.AddSingleton(x => KeyVaultProxy<IEventsKeyVault>.Create(GetKeyVaultConfiguration(x)));
        builder.AddSingleton<EventPublisher>();
    }

    private static KeyVaultConfiguration GetKeyVaultConfiguration(IServiceProvider serviceProvider)
    {
        var keyVaultData = serviceProvider.GetRequiredService<IIntegrationTestsConfiguration>().KeyVaultConfigSection;
        return new KeyVaultConfiguration(keyVaultData.KeyVaultUrl, keyVaultData.KeyVaultClientId, keyVaultData.KeyVaultSecret);
    }

    private static void RegisterDelegatingHandlers(IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<LogDelegatingHandler>();
        serviceCollection.AddTransient<LogDelegatingHandlerFactory>();
        serviceCollection.AddTransient<WxApiAuthenticationDelegatingHandler>();
        serviceCollection.AddSingleton<RetryPolicyHandlerFactory>();
    }

    private static void RegisterConfiguration(IServiceCollection builder)
    {
        builder.RegisterConfiguration<IntegrationTestsConfiguration, IIntegrationTestsConfiguration>(x =>
        {
            return x.WithSelfHostSource()
                    .WithServiceEndpoints<IIntegrationTestsConfiguration>()
                    .WithAdminConfiguration()
                    .WithRegions<IIntegrationTestsConfiguration>()
                    .WithAdminLoginData<IIntegrationTestsConfiguration>()
                    .WithKeyVault<ITestsKeyVault>()
                    .WithMainTableStorage<ITestsKeyVault>(keyVault => new StorageConnectionOptions
                    {
                        ConnectionString = keyVault.ConfigurationStorageConnectionString
                    })
                    .WithBackupBlobStorage<ITestsKeyVault>(keyVault => new StorageConnectionOptions
                    {
                        ConnectionString = keyVault.ConfigurationBackupStorageConnectionString
                    });
        });
    }

    private static void ConfigureServices(IServiceCollection serviceCollection)
    {
        var logger = new LoggerConfiguration().WriteTo.NUnitOutput(LogEventLevel.Debug)
            .MinimumLevel.Verbose()
            .Enrich.WithExceptionDetails()
            .CreateLogger();
        serviceCollection.AddSingleton<ILogger>(logger);

        serviceCollection.AddHttpClient();
        serviceCollection.AddSingleton<IFabricAddressResolver, FabricAddressResolver>();
        serviceCollection.RegisterListenerServices();
    }

    private static void RegisterApiClients(IServiceCollection serviceCollection)
    {
        var endpoints = _settings.Endpoints;

        serviceCollection.AddSingleton<AdminCredentialsProvider>();
        serviceCollection.AddRestEaseClient<ISignInApi>(endpoints.EnvApi)
            .AddTransientHttpErrorPolicy(builder => builder.OrResult(response => response.StatusCode is HttpStatusCode.Unauthorized)
                                                           .WaitAndRetryAsync(5, Timeouts.GetTimeout));
        serviceCollection.AddRestEaseClient<IOnfidoApi>(endpoints.OnfidoApi)
            .AddTransientHttpErrorPolicy(builder => builder.OrResult(response => response.StatusCode is HttpStatusCode.TooManyRequests)
                                                           .WaitAndRetryAsync(5, Timeouts.GetTimeout));

        serviceCollection.AddSingleton(provider =>
        {
            var credentialsProvider = provider.GetService<AdminCredentialsProvider>()!;
            var retryPolicyHandlerFactory = provider.GetService<RetryPolicyHandlerFactory>()!;
            var logDelegatingHandlerFactory = provider.GetService<LogDelegatingHandlerFactory>()!;
            var baseUri = new Uri(provider.GetServiceEndpoint(VerificationAdminApiFabricUrl, true));

            return new VerificationAdminApiClientFactory(baseUri, credentialsProvider, retryPolicyHandlerFactory, logDelegatingHandlerFactory);
        });
        serviceCollection.AddSingleton(provider =>
        {
            var credentialsProvider = provider.GetService<AdminCredentialsProvider>()!;
            var retryPolicyHandlerFactory = provider.GetService<RetryPolicyHandlerFactory>()!;
            var logDelegatingHandlerFactory = provider.GetService<LogDelegatingHandlerFactory>()!;
            var baseUri = new Uri(provider.GetServiceEndpoint(RisksAdminApiFabricUrl, true));

            return new RisksAdminApiClientFactory(baseUri, credentialsProvider, retryPolicyHandlerFactory, logDelegatingHandlerFactory);
        });
        serviceCollection.AddSingleton(provider =>
        {
            var credentialsProvider = provider.GetService<AdminCredentialsProvider>()!;
            var retryPolicyHandlerFactory = provider.GetService<RetryPolicyHandlerFactory>()!;
            var logDelegatingHandlerFactory = provider.GetService<LogDelegatingHandlerFactory>()!;
            var baseUri = new Uri(provider.GetServiceEndpoint(ProfileAdminApiFabricUrl, true));

            return new ProfileAdminApiClientFactory(baseUri, credentialsProvider, retryPolicyHandlerFactory, logDelegatingHandlerFactory);
        });

        serviceCollection.AddSingleton<IPublicClient>(provider =>
        {
            var retryPolicyFactory = provider.GetService<RetryPolicyHandlerFactory>()!;
            var retryPolicyHandler = retryPolicyFactory.Create();
            var logHandler = (DelegatingHandler)provider.GetService(typeof(LogDelegatingHandler))!;
            var wxApiProxyHandler = (DelegatingHandler)provider.GetService(typeof(WxApiAuthenticationDelegatingHandler))!;
            var client = HttpClientFactory.Create(wxApiProxyHandler, logHandler, retryPolicyHandler);

            return new VerificationPublicApiClient(endpoints.EnvApi, client);
        });

        serviceCollection.AddSingleton<ISurveyPublicClient>(provider =>
        {
            var retryPolicyFactory = provider.GetService<RetryPolicyHandlerFactory>()!;
            var retryPolicyHandler = retryPolicyFactory.Create();
            var logHandler = (DelegatingHandler)provider.GetService(typeof(LogDelegatingHandler))!;
            var wxApiProxyHandler = (DelegatingHandler)provider.GetService(typeof(WxApiAuthenticationDelegatingHandler))!;
            var client = HttpClientFactory.Create(wxApiProxyHandler, logHandler, retryPolicyHandler);

            return new SurveyPublicApiClient(endpoints.EnvApi, client);
        });

        serviceCollection.AddSingleton<IUserProfileClient>(provider =>
        {
            var retryPolicyFactory = provider.GetService<RetryPolicyHandlerFactory>()!;
            var retryPolicyHandler = retryPolicyFactory.Create();
            var logHandler = (DelegatingHandler)provider.GetService(typeof(LogDelegatingHandler))!;
            var wxApiProxyHandler = (DelegatingHandler)provider.GetService(typeof(WxApiAuthenticationDelegatingHandler))!;
            var client = HttpClientFactory.Create(wxApiProxyHandler, logHandler, retryPolicyHandler);

            return new UserProfileClient(endpoints.EnvApi, client);
        });

        serviceCollection.AddSingleton<IPreconditionClient>(provider =>
        {
            var retryPolicyFactory = provider.GetService<RetryPolicyHandlerFactory>()!;
            var retryPolicyHandler = retryPolicyFactory.Create();
            var logDelegatingHandlerFactory = provider.GetService<LogDelegatingHandlerFactory>()!;
            var logHandler = logDelegatingHandlerFactory.Create();
            var client = HttpClientFactory.Create(logHandler, retryPolicyHandler);
            var url = provider.GetServiceEndpoint(IntegrationTestsInternalApiFabricUrl, true);

            return new PreconditionClient(url, client);
        });
    }

    private static void RegisterSettingConfiguration(IServiceCollection serviceCollection)
    {
        var assemblyLocation = Assembly.GetAssembly(typeof(BaseTest))?.Location;
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(assemblyLocation))
            .AddJsonFile($"appsettings.json", true);
        _configuration = configuration.Build();

        serviceCollection.AddSingleton(_configuration);

        _settings = new Settings();
        _configuration.Bind(_settings);
        serviceCollection.AddSingleton(_settings);
        serviceCollection.AddSingleton(_settings.User);
    }
}
