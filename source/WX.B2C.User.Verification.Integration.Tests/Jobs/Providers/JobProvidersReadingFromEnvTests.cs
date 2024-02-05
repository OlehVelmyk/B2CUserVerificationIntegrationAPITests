using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using WX.B2C.User.Verification.BlobStorage.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Profile;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess;
using WX.B2C.User.Verification.Worker.Jobs.Providers;
using WX.B2C.User.Verification.Worker.Jobs.Providers.CollectionSteps;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Integration.Tests.Jobs.Providers
{
    [TestFixture]
    [Explicit]
    public class JobProvidersReadingFromEnvTests : BaseIntegrationTest, IResolver
    {
        protected override void RegisterModules(ContainerBuilder containerBuilder)
        {
            base.RegisterModules(containerBuilder);
            containerBuilder.RegisterDbQueryFactory();

            RegisterKeyVault<IUserVerificationKeyVault>(containerBuilder);
            RegisterKeyVault<IB2CUserVerificationKeyVault>(containerBuilder);

            containerBuilder.Register(с =>
                            {
                                var config = Substitute.For<IAppConfig>();
                                var dbConnectionString = с.Resolve<IB2CUserVerificationKeyVault>().DbConnectionString;
                                config.DbConnectionString.Returns(dbConnectionString);
                                return config;
                            })
                            .As<IAppConfig>();

            containerBuilder.RegisterType<UserVerificationQueryFactory>().As<IUserVerificationQueryFactory>().SingleInstance();
        }

        /// <summary>
        /// Given initialized provider
        /// And real database on env
        /// When providers read total
        /// And providers read values
        /// Then no exceptions must be thrown
        /// And Total must equals count of elements
        /// </summary>
        [Test]
        [TestCaseSource(nameof(JobProvidersSource))]
        public async Task ShouldProviderReadTotalAndValues(JobProviderTestCase providerTestCase)
        {
            var total = await providerTestCase.GetTotal(this);
            var actualTotal = 0;

            await foreach (var batch in providerTestCase.GetObjects(this))
            {
                actualTotal += batch.Count(o => o is not null);
            }

            actualTotal.Should().Be(total);
        }

        public static IEnumerable<JobProviderTestCase> JobProvidersSource()
        {
            var csvBlobStorage = Substitute.For<ICsvBlobStorage>();
            
            var batchSize = 3000;
            //TODO Would be nice to have some real user ids from env
            var users = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

            var setting = new UserBatchJobSettings { ReadingBatchSize = batchSize };
            yield return JobProviderTestCase.Create(factory => new ProfileDataExistenceProvider(factory,csvBlobStorage),
                                                    new CollectionStepsJobSettings { ReadingBatchSize = batchSize });
            yield return JobProviderTestCase.Create(factory => new ApplicantDataProvider(factory, csvBlobStorage),
                                                    new SelfieJobSettings { ReadingBatchSize = batchSize });
            yield return JobProviderTestCase.Create(factory => new ApplicationDataProvider(factory), setting);
            yield return JobProviderTestCase.Create(factory => new FileDataProvider(factory),
                                                    new FileValidationJobSettings { ReadingBatchSize = batchSize });
            yield return JobProviderTestCase.Create(factory => new OnfidoFileDataProvider(factory),
                                                    new OnfidoDocumentOcrJobSetting { ReadingBatchSize = batchSize });
            yield return JobProviderTestCase.Create(factory => new UsaApplicationDataProvider(factory),
                                                    new FraudScreeningTaskJobSettings { ReadingBatchSize = batchSize });

            yield return JobProviderTestCase.Create(factory => new TaxResidenceProvider(factory),
                                                    new TaxResidenceJobSetting { ReadingBatchSize = batchSize });
            yield return JobProviderTestCase.Create(factory => new ProofOfAddressProvider(factory),
                                                    new ProofOfAddressJobSetting { ReadingBatchSize = batchSize });

            yield return JobProviderTestCase.Create(resolver => new DocumentsChecksProvider(resolver.Resolve<IUserVerificationKeyVault>(),
                                                                                            csvBlobStorage),
                                                    new CollectionStepsJobSettings { ReadingBatchSize = batchSize});
            yield return JobProviderTestCase.Create(resolver => new UserSurveyChecksProvider(resolver.Resolve<IUserVerificationKeyVault>(), csvBlobStorage),
                                                    new CollectionStepsJobSettings { ReadingBatchSize = batchSize });

            //TODO provider doesn't work
            //yield return JobProviderTestCase.Create(resolver => new ProofOfFundsCheckDataProvider(resolver.Resolve<IUserVerificationKeyVault>()),
            //                                        new UserBatchJobSettings() { ReadingBatchSize = envDbBatchSize });

            yield return JobProviderTestCase.Create(
            resolver => new FinancialConditionProvider(resolver.Resolve<IUserVerificationKeyVault>()),
            new FinancialConditionJobSetting { ReadingBatchSize = batchSize });

            var accountAlertJobConfig = new AccountAlertJobConfig
            {
                ApplicationState = ApplicationState.Approved,
                ExcludedCountries = new[] { "GB" },
                Periods = new[]
                {
                    new AlertPeriod { AccountAge = 1, OverallTurnover = 10, RiskLevel = RiskLevel.Low }
                }
            };
            yield return JobProviderTestCase.Create(factory => new AccountAlertInfoProvider(accountAlertJobConfig, factory), setting);
        }

        public new T Resolve<T>()
        {
            return base.Resolve<T>();
        }
    }

    public interface IResolver
    {
        public T Resolve<T>();
    }
}