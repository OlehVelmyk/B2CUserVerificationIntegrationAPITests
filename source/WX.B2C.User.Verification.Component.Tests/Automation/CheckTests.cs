using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck;
using NUnit.Framework;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Arbitraries;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Fixtures;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Component.Tests.Helpers;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Options;
using WX.B2C.User.Verification.Component.Tests.Providers;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Events.Internal.Extensions;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Component.Tests.Automation
{
    internal class CheckTests : BaseComponentTest
    {
        private IReadOnlyCollection<CheckType> _excludedChecks = new[]
        {
            CheckType.IpMatch,
            CheckType.IdentityEnhanced,
            CheckType.NameAndDoBDuplication,
            CheckType.RiskListsScreening
        };

        private ApplicationFixture _applicationFixture;
        private ProfileFixture _profileFixture;
        private VerificationDetailsFixture _verificationDetailsFixture;
        private CollectionStepsFixture _collectionStepsFixture;
        private ChecksFixture _checksFixture;
        private ExternalProfileFixture _externalProfileFixture;
        private EventsFixture _eventsFixture;
        private AdminApiClientFactory _adminApiClientFactory;
        private AdministratorFactory _adminFactory;
        private StepVariantComparer _stepVariantComparer;
        private ICheckProvider _checkProvider;


        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _applicationFixture = Resolve<ApplicationFixture>();
            _profileFixture = Resolve<ProfileFixture>();
            _verificationDetailsFixture = Resolve<VerificationDetailsFixture>();
            _collectionStepsFixture = Resolve<CollectionStepsFixture>();
            _checksFixture = Resolve<ChecksFixture>();
            _externalProfileFixture = Resolve<ExternalProfileFixture>();
            _eventsFixture = Resolve<EventsFixture>();
            _adminApiClientFactory = Resolve<AdminApiClientFactory>();
            _adminFactory = Resolve<AdministratorFactory>();
            _stepVariantComparer = Resolve<StepVariantComparer>();
            _checkProvider = Resolve<ICheckProvider>();

            Arb.Register<IdDocumentNumberArbitrary>();
            Arb.Register<FullNameArbitrary>();
            Arb.Register<AddressArbitrary>();
            Arb.Register<UserInfoArbitrary>();
            Arb.Register<GbUserInfoArbitrary>();
            Arb.Register<UsUserInfoArbitrary>();
            Arb.Register<EeaUserInfoArbitrary>();
            Arb.Register<NotGlobalUserInfoArbitrary>();
        }

        /// <summary>
        /// Scenario: Start check
        /// Given user with pending check
        /// When last collection step is completed
        /// Then check is started
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldStartCheck_WhenCompleteLastStep(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var checks = (await adminClient.Checks.GetAllAsync(userId))
                            .Where(c => c.State == CheckState.Pending)
                            .Where(c => c.Variant.Provider is not CheckProviderType.Onfido)
                            .Where(c => !c.Type.In(_excludedChecks))
                            .OrderBy(c => c.Type).ToArray();
            var check = FakerFactory.Create(seed).PickRandom(checks);
            var requiredData = _checkProvider.GetRequiredData(check.Variant.Id);

            var steps = await adminClient.CollectionStep.GetAllAsync(userId);
            var requiredSteps = steps.Where(step => step.State != CollectionStepState.Completed)
                                     .Where(step => requiredData.Contains(step.Variant, _stepVariantComparer))
                                     .OrderBy(step => step.Variant.Name)
                                     .ToArray();

            var step = requiredSteps.First();
            await requiredSteps.Skip(1).ForeachConsistently(s => _collectionStepsFixture.CompleteAsync(userId, s.Id, seed));

            // Act
            await _collectionStepsFixture.CompleteAsync(userId, step.Id, seed);

            // Assert
            _eventsFixture.ShouldExistSingle<CheckStartedEvent>(e => e.EventArgs.CheckId == check.Id);
            _eventsFixture.ShouldExistSingle<CheckCompletedEvent>(e => e.EventArgs.CheckId == check.Id);
        }

        /// <summary>
        /// Scenario: Run onfido checks
        /// Given user with Onfido checks
        /// When complete required data for all of them
        /// Then Onfido checks is run
        /// And events are raised
        /// </summary>
        [Theory]
        public async Task ShouldRunOnfidoChecks(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var checks = await adminClient.ExecuteUntilAsync(
                client => client.Checks.GetAllAsync(userId),
                checks => checks.Any(check => check.Variant.Provider is CheckProviderType.Onfido));

            var onfidoChecks = checks.Where(check => check.Variant.Provider is CheckProviderType.Onfido);
            var requiredData = onfidoChecks.Select(check => _checkProvider.GetRequiredData(check.Variant.Id)).Flatten();

            var steps = await adminClient.CollectionStep.GetAllAsync(userId);
            var requiredSteps = steps.Where(step => requiredData.Any(variant => _stepVariantComparer.Equals(step.Variant, variant)));

            // Act
            await _collectionStepsFixture.CompleteAllAsync(userId, requiredSteps.Select(step => step.Id), false, seed);

            // Assert
            onfidoChecks.Foreach(check => _eventsFixture.ShouldExistSingle<CheckCompletedEvent>(e => e.EventArgs.CheckId == check.Id));
        }

        /// <summary>
        /// Scenario: Run successfully onfido check
        /// Given user with Onfido checks
        /// When run checks
        /// Then one is completed with result 'Passed'
        /// However other onfido checks completed as 'Failed'
        /// And events are raised
        /// </summary>
        [Theory]
        public async Task ShouldSuccessfullyRunOnfidoCheck_WhenOtherFailed(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var applicantId = await _externalProfileFixture.GetApplicantIdAsync(userId);
            var documentCheckOption = OnfidoCheckOption.Passed(CheckType.IdentityDocument);
            var faceDuplicationcheckOption = OnfidoCheckOption.Failed(CheckType.FaceDuplication);
            var facialSimiliarityCheckOption = OnfidoCheckOption.Failed(CheckType.FacialSimilarity);
            var checkOptionArray = new[] { documentCheckOption, faceDuplicationcheckOption, facialSimiliarityCheckOption };
            var checkOptions = OnfidoGroupedCheckOptions.Create(applicantId, checkOptionArray);
            await OnfidoImposter.ConfigureCheckAsync(checkOptions);

            var checks = await adminClient.ExecuteUntilAsync(
                client => client.Checks.GetAllAsync(userId),
                checks => checks.Any(check => check.Variant.Provider is CheckProviderType.Onfido));

            var onfidoChecks = checks.Where(check => check.Variant.Provider is CheckProviderType.Onfido);
            var requiredData = onfidoChecks.Select(check => _checkProvider.GetRequiredData(check.Variant.Id)).Flatten();

            var steps = await adminClient.CollectionStep.GetAllAsync(userId);
            var requiredSteps = steps.Where(step => requiredData.Any(variant => _stepVariantComparer.Equals(step.Variant, variant)));

            // Act
            await _collectionStepsFixture.CompleteAllAsync(userId, requiredSteps.Select(step => step.Id), false, seed);

            // Assert
            onfidoChecks.Foreach(check => _eventsFixture.ShouldExistSingle<CheckCompletedEvent>(e => e.EventArgs.CheckId == check.Id));

            var actualChecks = await adminClient.Checks.GetAllAsync(userId);
            actualChecks.Should().Contain(check => check.Type == CheckType.IdentityDocument && check.Result == CheckResult.Passed);
            actualChecks.Should().Contain(check => check.Type == CheckType.FaceDuplication && check.Result == CheckResult.Failed);
            actualChecks.Should().Contain(check => check.Type == CheckType.FacialSimilarity && check.Result == CheckResult.Failed);
        }

        /// <summary>
        /// Scenario: Not run onfido single check
        /// Given user with Onfido checks
        /// When complete required data for onfido check
        /// And required data for other onfido checks are not completed
        /// Then single Onfido check is not run
        /// And event is not raised
        /// </summary>
        [Theory]
        public async Task ShouldNotRunOnfidoSingleCheck(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var checks = await adminClient.ExecuteUntilAsync(
                client => client.Checks.GetAllAsync(userId),
                checks => checks.Any(check => check.Type is CheckType.IdentityDocument));

            var check = checks.First(check => check.Type is CheckType.IdentityDocument);
            var requiredData = _checkProvider.GetRequiredData(check.Variant.Id);

            var steps = await adminClient.CollectionStep.GetAllAsync(userId);
            var requiredSteps = steps.Where(step => requiredData.Any(variant => _stepVariantComparer.Equals(step.Variant, variant)));

            // Act
            await _collectionStepsFixture.CompleteAllAsync(userId, requiredSteps.Select(step => step.Id), false, seed);

            // Assert
            _eventsFixture.ShouldNotExist<CheckCompletedEvent>(e => e.EventArgs.CheckId == check.Id);
        }

        /// <summary>
        /// Scenario: Run onfido single check
        /// Given user with Onfido checks
        /// When admin requests new check
        /// And required data are completed
        /// Then single Onfido check is run
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldRunOnfidoSingleCheck(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var checks = await adminClient.ExecuteUntilAsync(
                client => client.Checks.GetAllAsync(userId),
                checks => checks.Any(check => check.Type is CheckType.IdentityDocument));

            var check = checks.First(check => check.Type is CheckType.IdentityDocument);
            var requiredData = _checkProvider.GetRequiredData(check.Variant.Id);

            var steps = await adminClient.CollectionStep.GetAllAsync(userId);
            var requiredSteps = steps.Where(step => requiredData.Any(variant => _stepVariantComparer.Equals(step.Variant, variant)));
            await _collectionStepsFixture.CompleteAllAsync(userId, requiredSteps.Select(step => step.Id), false, seed);

            // Act
            var checkId = await _checksFixture.RequestAsync(userId, check.Variant.Id);

            // Assert
            _eventsFixture.ShouldExist<CheckCompletedEvent>(e => e.EventArgs.CheckId == checkId);
        }

        /// <summary>
        /// Scenario: Not run high priority check 
        /// Given user with different priority checks
        /// When try to run more priority check before less ones
        /// Then check is not run
        /// And event is not raised
        /// </summary>
        [Theory]
        public async Task ShouldNotRunCheck_WhenHighPriority(UsUserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var checks = await adminClient.ExecuteUntilAsync(
                client => client.Checks.GetAllAsync(userId),
                checks => checks.Any(IsFraudScreening));

            var check = checks.First(IsFraudScreening);
            var requiredData = _checkProvider.GetRequiredData(check.Variant.Id);

            var steps = await adminClient.CollectionStep.GetAllAsync(userId);
            var requiredSteps = steps.Where(step => requiredData.Any(variant => _stepVariantComparer.Equals(step.Variant, variant)));

            // Act
            await _collectionStepsFixture.CompleteAllAsync(userId, requiredSteps.Select(step => step.Id), false, seed);

            // Assert
            _eventsFixture.ShouldNotExist<CheckCompletedEvent>(e => e.EventArgs.CheckId == check.Id);

            static bool IsFraudScreening(CheckDto check) => check.Type is CheckType.FraudScreening;
        }

        /// <summary>
        /// Scenario: Instruct check (Verification details)
        /// Given user with completed checks
        /// When verification details is updated
        /// And updated data is required data for some check
        /// Then check is instructed and requested 
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldInstructCheck_WhenVerificationDetailsUpdated(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var checks = await adminClient.ExecuteUntilAsync(
                client => client.Checks.GetAllAsync(userId),
                checks => checks.Any(check => check.Type == CheckType.IdDocNumberDuplication) &&
                          checks.Any(check => check.Type == CheckType.IdentityDocument));
            var check = checks.First(check => check.Type == CheckType.IdDocNumberDuplication);

            await _checksFixture.CompleteAsync(userId, check.Variant.Id, seed, null);

            // Act
            var documentCheck = checks.First(check => check.Type == CheckType.IdentityDocument);
            await _checksFixture.CompleteAsync(userId, documentCheck.Variant.Id, seed, null);

            // Assert
            _eventsFixture.ShouldExist<CheckCreatedEvent>(
                e => e.EventArgs.UserId == userId && e.EventArgs.VariantId == check.Variant.Id && e.EventArgs.CheckId != check.Id);
        }

        /// <summary>
        /// Scenario: Not instruct check 
        /// Given user with completed checks
        /// When verification details is updated by admin
        /// And updated data is required data for some check
        /// Then check is not instructed 
        /// And event is not raised
        /// </summary>
        [Theory]
        public async Task ShouldNotInstructCheck_WhenAdminUpdateVerificationDetails(UserInfo userInfo, IdDocumentNumber idDocNumber, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var checks = await adminClient.ExecuteUntilAsync(
                client => client.Checks.GetAllAsync(userId),
                checks => checks.Any(check => check.Type == CheckType.IdDocNumberDuplication));
            var check = checks.First(check => check.Type == CheckType.IdDocNumberDuplication);

            await _checksFixture.CompleteAsync(userId, check.Variant.Id, seed, null);

            // Act
            await _verificationDetailsFixture.UpdateByAdminAsync(userId, builder => builder.With(idDocNumber));

            // Assert
            _eventsFixture.ShouldNotExist<CheckCreatedEvent>(
                e => e.EventArgs.UserId == userId && e.EventArgs.VariantId == check.Variant.Id && e.EventArgs.CheckId != check.Id);
        }

        /// <summary>
        /// Scenario: Instruct check (Personal details)
        /// Given user with completed checks
        /// When personal details is updated
        /// And updated data is required data for some check
        /// Then check is instructed and requested 
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldInstructCheck_WhenPersonalDetailsUpdated(UserInfo userInfo, FullName fullName)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var checks = await adminClient.ExecuteUntilAsync(
                client => client.Checks.GetAllAsync(userId),
                checks => checks.Any(IsNameAndDoBCheckCompleted));
            var check = checks.First(check => check.Type == CheckType.NameAndDoBDuplication);

            // Arrange 
            var newName = new UserInfo
            {
                UserId = userId,
                FirstName = fullName.FirstName,
                LastName = fullName.LastName
            };

            // Act
            await _profileFixture.CreateAsync(newName);

            // Assert
            _eventsFixture.ShouldExistSingle<CheckCreatedEvent>(
                e => e.EventArgs.UserId == userId && e.EventArgs.VariantId == check.Variant.Id && e.EventArgs.CheckId != check.Id);

            bool IsNameAndDoBCheckCompleted(CheckDto check) =>
                check.Type == CheckType.NameAndDoBDuplication && check.State is CheckState.Complete;
        }

        /// <summary>
        /// Scenario: Instruct check (Collection step requested)
        /// Given user with completed checks
        /// When admin request new collection step
        /// Then check is instructed and requested 
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldInstructCheck_WhenAdminRequestCollectionStep(UserInfo userInfo, IdDocumentNumber idDocNumber, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var checks = await adminClient.Checks.GetAllAsync(userId);
            checks = checks.Where(check => check.State is CheckState.Pending).ToArray();
            var faker = FakerFactory.Create(seed);
            var check = faker.PickRandom(checks);

            await _checksFixture.CompleteAsync(userId, check.Variant.Id, seed, null);

            var requiredData = _checkProvider.GetRequiredData(check.Variant.Id);
            var stepVariant = faker.PickRandom(requiredData);

            // Act
            await _collectionStepsFixture.RequestAsync(userId, stepVariant, false, false);

            // Assert
            _eventsFixture.ShouldExist<CheckCreatedEvent>(
                e => e.EventArgs.UserId == userId && e.EventArgs.VariantId == check.Variant.Id && e.EventArgs.CheckId != check.Id);
        }

        /// <summary>
        /// Scenario: Extract data from passed completed check
        /// Given user with pending check
        /// When check is completed with result 'Passed'
        /// Then verification details is extracted from check
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldExtractVerificationDetailsFromPassedCheck(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var checks = await adminClient.ExecuteUntilAsync(
                client => client.Checks.GetAllAsync(userId),
                checks => checks.Any(check => check.Type == CheckType.IdentityDocument));
            var check = checks.First(check => check.Type == CheckType.IdentityDocument);

            // Act
            await _checksFixture.CompleteAsync(userId, check.Variant.Id, seed, null);

            // Assert
            _eventsFixture.ShouldExistSingle<VerificationDetailsUpdatedEvent>(IsVerificationDetailsUpdated);

            bool IsVerificationDetailsUpdated(VerificationDetailsUpdatedEvent e) =>
                e.EventArgs.UserId == userId &&
                e.EventArgs.Changes.Find<IdDocumentNumber>(VerificationDetails.IdDocumentNumber) is not null &&
                e.EventArgs.Changes.Find<string>(VerificationDetails.Nationality) is not null;
        }

        /// <summary>
        /// Scenario: Extract data from failed completed check
        /// Given user with pending check
        /// When check is completed with result 'Failed'
        /// Then verification details is extracted from check
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldExtractVerificationDetailsFromFailedCheck(NotGlobalUserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            userInfo.IpAddress = IpAddresses.FailAddress;

            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var checks = await adminClient.ExecuteUntilAsync(
                client => client.Checks.GetAllAsync(userId),
                checks => checks.Any(check => check.Type == CheckType.IpMatch));
            var check = checks.First(check => check.Type == CheckType.IpMatch);

            // Act
            await _checksFixture.CompleteAsync(userId, check.Variant.Id, seed, null);

            // Assert
            _eventsFixture.ShouldExistSingle<CheckCompletedEvent>(
                e => e.EventArgs.CheckId == check.Id && e.EventArgs.Result.ToString() == CheckResult.Failed.ToString());
            _eventsFixture.ShouldExistSingle<VerificationDetailsUpdatedEvent>(
                e => e.EventArgs.UserId == userId && e.EventArgs.Changes.Find<bool?>(VerificationDetails.IsIpMatched) is not null);
        }

        /// <summary>
        /// Scenario: Not extract data from failed completed check
        /// Given user with pending check
        /// When check is completed with result 'Failed'
        /// And check output review policy is 'FailReviewRequired'
        /// Then verification details is not extracted from check
        /// And event is not raised
        /// </summary>
        [Theory]
        public async Task ShouldNotExtractVerificationDetailsFromFailedCheck(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var applicantId = await _externalProfileFixture.GetApplicantIdAsync(userId);
            var checkOption = OnfidoCheckOption.Failed(CheckType.IdentityDocument);
            var checkOptions = OnfidoGroupedCheckOptions.Create(applicantId, checkOption);
            await OnfidoImposter.ConfigureCheckAsync(checkOptions);

            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            var checks = await adminClient.ExecuteUntilAsync(
                client => client.Checks.GetAllAsync(userId),
                checks => checks.Any(check => check.Type == CheckType.IdentityDocument));
            var check = checks.First(check => check.Type == CheckType.IdentityDocument);

            // Act
            await _checksFixture.CompleteAsync(userId, check.Variant.Id, seed, null);

            // Assert
            _eventsFixture.ShouldNotExist<VerificationDetailsUpdatedEvent>(IsVerificationDetailsUpdated);

            bool IsVerificationDetailsUpdated(VerificationDetailsUpdatedEvent e) =>
                e.EventArgs.UserId == userId &&
                e.EventArgs.Changes.Find<IdDocumentNumber>(VerificationDetails.IdDocumentNumber) is not null &&
                e.EventArgs.Changes.Find<string>(VerificationDetails.Nationality) is not null;
        }

        /// <summary>
        /// Scenario: Not instruct check according to run policy
        /// Given user with a few sequential failed checks
        /// When instruct check which reaches max run attempts
        /// Then check is not requested again
        /// And event is not raised
        /// </summary>
        [Theory]
        public async Task ShouldNotInstructCheckAccordingToRunPolicy(GbUserInfo userInfo, Address address, Seed seed)
        {
            // Given
            const int MaxRunAttempts = 5;
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var applicantId = await _externalProfileFixture.GetApplicantIdAsync(userId);
            var checkOption = OnfidoCheckOption.Failed(CheckType.IdentityEnhanced);
            var checkOptions = OnfidoGroupedCheckOptions.Create(applicantId, checkOption);
            await OnfidoImposter.ConfigureCheckAsync(checkOptions);

            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            var checks = await adminClient.ExecuteUntilAsync(
                client => client.Checks.GetAllAsync(userId),
                checks => checks.Any(check => check.Type == CheckType.IdentityEnhanced));
            var check = checks.First(check => check.Type == CheckType.IdentityEnhanced);

            await Enumerable.Range(1, MaxRunAttempts - 1)
                            .ForeachConsistently(_ => _checksFixture.CompleteAsync(userId, check.Variant.Id, seed, null));

            // Act
            await _profileFixture.UpdateAsync(userId, address);

            // Assert
            _eventsFixture.ShouldNotExist<CheckCreatedEvent>(
                e => e.CorrelationId == adminClient.CorrelationId && e.EventArgs.VariantId == check.Variant.Id);
        }

        /// <summary>
        /// Scenario: Not instruct check pending check
        /// Given user with a pending check
        /// When instruct same check 
        /// Then check is not requested again
        /// And event is not raised
        /// </summary>
        [Theory]
        public async Task ShouldNotInstructPendingCheck(UserInfo userInfo, IdDocumentNumber idDocumentNumber)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var variantId = new Guid("BB30DACB-F8A0-477C-941A-FB0C71C0297A");
            await _checksFixture.RequestAsync(userId, variantId);

            // Act
            var correlationId = Guid.NewGuid();
            await _verificationDetailsFixture.UpdateByAdminAsync(userId, builder => builder.With(idDocumentNumber), correlationId);

            // Assert
            _eventsFixture.ShouldNotExist<CheckCreatedEvent>(
                e => e.CorrelationId == correlationId && e.EventArgs.VariantId == variantId);
        }

        /// <summary> 
        /// Scenario: Start check (Absent requested data)
        /// Given user
        /// When request check 
        /// And required data for check are not submitted
        /// And required data are requested
        /// Then check is created
        /// And check is not started
        /// And event about creation of check is raised
        /// And event about starting is not raised
        /// </summary>
        [Theory]
        public async Task ShouldNotRunCheck_WhenRequiredDataAbsent(UserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Act
            var variantId = new Guid("A9E0048B-0F6B-44F0-8A22-703DD86BA05E");
            var checkId = await _checksFixture.RequestAsync(userId, variantId);

            // Assert
            _eventsFixture.ShouldNotExist<CheckStartedEvent>(e => e.EventArgs.CheckId == checkId); // VerificationDetails.TaxResidence is absent
        }

        /// <summary> 
        /// Scenario: Start check (Absent requested data)
        /// Given user
        /// When request check 
        /// And required data for check are not submitted
        /// And required data are not requested
        /// Then check is created
        /// And check is started
        /// And check state is 'Error'
        /// And event about creation of check is raised
        /// And event about starting is raised
        /// And event about error in check is raised
        /// </summary>
        [Theory, Ignore("TODO: think how to implement test case")]
        public async Task ShouldRunCheck_WhenRequiredDataAbsent(EeaUserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            // Act
            var variantId = new Guid("0E3B0522-8330-487F-A35C-38CC6022812A");
            var checkId = await _checksFixture.RequestAsync(userId, variantId);

            // Assert
            _eventsFixture.ShouldNotExist<CheckStartedEvent>(e => e.EventArgs.CheckId == checkId);
            _eventsFixture.ShouldExist<CheckErrorOccuredEvent>(e => e.EventArgs.CheckId == checkId); // Document.Selfie.Video is absent
        }

        /// <summary>
        /// Scenario: Run check (InReview step)
        /// Given user with a pending check
        /// And some required data for check is in review
        /// When complete last step for check
        /// Then check is not started
        /// And event is not raised
        /// </summary>
        /// <remarks>
        /// By default required data for check are not review needed
        /// </remarks>
        [Theory, Ignore("TODO: Implement logic on service")]
        public async Task ShouldNotRunCheckWithInReviewData(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var steps = (await adminClient.CollectionStep.GetAllAsync(userId))
                                .Where(step => step.State != CollectionStepState.Completed)
                                .OrderBy(step => step.Variant.Name)
                                .ToArray();
            var checks = (await adminClient.Checks.GetAllAsync(userId))
                                .Where(c => c.State == CheckState.Pending)
                                .Where(c => !c.Type.In(_excludedChecks))
                                .OrderBy(c => c.Type).ToArray();
            var checksWithSteps = checks.Select(c => (check: c, requestedSteps: GetRequestedSteps(c)))
                                        .Where(tuple => tuple.requestedSteps.Length >= 2);

            var (check, requiredSteps) = FakerFactory.Create(seed).PickRandom(checksWithSteps);

            var step = requiredSteps.First();
            await _collectionStepsFixture.UpdateAsync(userId, step.Id, true, true);
            await _collectionStepsFixture.MoveInReviewAsync(userId, step.Id, seed);

            // Act
            await requiredSteps.Skip(1).ForeachConsistently(s => _collectionStepsFixture.CompleteAsync(userId, s.Id, seed));

            // Assert
            _eventsFixture.ShouldNotExist<CheckStartedEvent>(e => e.EventArgs.CheckId == check.Id);

            CollectionStepDto[] GetRequestedSteps(CheckDto check)
            {
                var requiredData = _checkProvider.GetRequiredData(check.Variant.Id);
                return steps.Where(step => requiredData.Contains(step.Variant, _stepVariantComparer))
                            .ToArray();
            }
        }

        /// <summary>
        /// Scenario: Run check (Rejected step)
        /// Given user with a pending check
        /// And some required data for check is rejected
        /// When complete last step for check
        /// Then check is not started
        /// And event is not raised
        /// </summary>
        /// <remarks>
        /// By default required data for check are not review needed
        /// </remarks>
        [Theory, Ignore("TODO: Implement logic on service")]
        public async Task ShouldNotRunCheckWithRejectedData(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var steps = (await adminClient.CollectionStep.GetAllAsync(userId))
                                .Where(step => step.State != CollectionStepState.Completed)
                                .OrderBy(step => step.Variant.Name)
                                .ToArray();
            var checks = (await adminClient.Checks.GetAllAsync(userId))
                                .Where(c => c.State == CheckState.Pending)
                                .Where(c => !c.Type.In(_excludedChecks))
                                .OrderBy(c => c.Type).ToArray();
            var checksWithSteps = checks.Select(c => (check: c, requestedSteps: GetRequestedSteps(c)))
                                        .Where(tuple => tuple.requestedSteps.Length >= 2);

            var (check, requiredSteps) = FakerFactory.Create(seed).PickRandom(checksWithSteps);

            var step = requiredSteps.First();
            await _collectionStepsFixture.UpdateAsync(userId, step.Id, true, true);
            await _collectionStepsFixture.MoveInReviewAsync(userId, step.Id, seed);
            await _collectionStepsFixture.ReviewAsync(userId, step.Id, CollectionStepReviewResult.Rejected);

            // Act
            await requiredSteps.Skip(1).ForeachConsistently(s => _collectionStepsFixture.CompleteAsync(userId, s.Id, seed));

            // Assert
            _eventsFixture.ShouldNotExist<CheckStartedEvent>(e => e.EventArgs.CheckId == check.Id);

            CollectionStepDto[] GetRequestedSteps(CheckDto check)
            {
                var requiredData = _checkProvider.GetRequiredData(check.Variant.Id);
                return steps.Where(step => requiredData.Contains(step.Variant, _stepVariantComparer))
                            .ToArray();
            }
        }

        /// <summary>
        /// Scenario: Run check (Rejected step)
        /// Given user with a pending check
        /// And all required data for check is completed except one
        /// And the last one is review needed
        /// When review last step as rejected
        /// Then check is not started
        /// And event is not raised
        /// </summary>
        /// <remarks>
        /// By default required data for check are not review needed
        /// </remarks>
        [Theory]
        public async Task ShouldNotRunCheckWhenLastStepIsRejected(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);
            var checks = (await adminClient.Checks.GetAllAsync(userId))
                            .Where(c => c.State == CheckState.Pending)
                            .Where(c => !c.Type.In(_excludedChecks))
                            .OrderBy(c => c.Type).ToArray();
            var check = FakerFactory.Create(seed).PickRandom(checks);
            var requiredData = _checkProvider.GetRequiredData(check.Variant.Id);

            var steps = await adminClient.CollectionStep.GetAllAsync(userId);
            var requiredSteps = steps.Where(step => step.State != CollectionStepState.Completed)
                                     .Where(step => requiredData.Contains(step.Variant, _stepVariantComparer))
                                     .OrderBy(step => step.Variant.Name)
                                     .ToArray();

            var step = requiredSteps.First();
            await _collectionStepsFixture.UpdateAsync(userId, step.Id, true, true);

            await requiredSteps.Skip(1).ForeachConsistently(s => _collectionStepsFixture.CompleteAsync(userId, s.Id, seed));

            // Act
            await _collectionStepsFixture.MoveInReviewAsync(userId, step.Id, seed);
            await _collectionStepsFixture.ReviewAsync(userId, step.Id, CollectionStepReviewResult.Rejected);

            // Assert
            _eventsFixture.ShouldNotExist<CheckStartedEvent>(e => e.EventArgs.CheckId == check.Id);
        }

        /// <summary>
        /// Scenario: Automatic re-request check
        /// Given user with pending check
        /// When check is completed with 'Failed' result
        /// And check decision is 'Resubmit'
        /// Then new same check is requested
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldReRequestCheck(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();

            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            await _applicationFixture.BuildApplicationAsync(userInfo);

            var applicantId = await _externalProfileFixture.GetApplicantIdAsync(userId);
            var checkOption = OnfidoCheckOption.Failed(CheckType.IdentityDocument, CheckDecisions.Resubmit);
            var checkOptions = OnfidoGroupedCheckOptions.Create(applicantId, checkOption);
            await OnfidoImposter.ConfigureCheckAsync(checkOptions);

            var checks = await adminClient.ExecuteUntilAsync(
                client => client.Checks.GetAllAsync(userId),
                checks => checks.Any(check => check.Type == CheckType.IdentityDocument));
            var check = checks.First(check => check.Type == CheckType.IdentityDocument);

            // Act
            await _checksFixture.CompleteAsync(userId, check.Id, seed);

            // Assert
            _eventsFixture.ShouldExistSingle<CheckCreatedEvent>(
                e => e.EventArgs.UserId == userId && e.EventArgs.VariantId == check.Variant.Id && e.EventArgs.CheckId != check.Id);
        }
    }
}
