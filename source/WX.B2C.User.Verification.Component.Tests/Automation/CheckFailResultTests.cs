using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using FsCheck;
using NUnit.Framework;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Api.Public.Client;
using WX.B2C.User.Verification.Component.Tests.Arbitraries;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Fixtures;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Component.Tests.Mountebank.Options;
using WX.B2C.User.Verification.Events.Internal.Events;
using Documents = WX.B2C.User.Verification.Component.Tests.Constants.Documents;

namespace WX.B2C.User.Verification.Component.Tests.Automation
{
    internal class CheckFailResultTests : BaseComponentTest
    {
        private ApplicationFixture _applicationFixture;
        private ProfileFixture _profileFixture;
        private VerificationDetailsFixture _verificationDetailsFixture;
        private ChecksFixture _checksFixture;
        private ExternalProfileFixture _externalProfileFixture;
        private EventsFixture _eventsFixture;
        private AdminApiClientFactory _adminApiClientFactory;
        private AdministratorFactory _adminFactory;
        private PublicApiClientFactory _publicApiClientFactory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _applicationFixture = Resolve<ApplicationFixture>();
            _profileFixture = Resolve<ProfileFixture>();
            _verificationDetailsFixture = Resolve<VerificationDetailsFixture>();
            _checksFixture = Resolve<ChecksFixture>();
            _externalProfileFixture = Resolve<ExternalProfileFixture>();
            _eventsFixture = Resolve<EventsFixture>();
            _adminApiClientFactory = Resolve<AdminApiClientFactory>();
            _adminFactory = Resolve<AdministratorFactory>();
            _publicApiClientFactory = Resolve<PublicApiClientFactory>();

            Arb.Register<UserInfoArbitrary>();
            Arb.Register<NotGbUserInfoArbitrary>();
            Arb.Register<GbUserInfoArbitrary>();
            Arb.Register<UsUserInfoArbitrary>();
            Arb.Register<NotUsUserInfoArbitrary>();
        }

        /// <summary>
        /// Scenario: Request PoA step
        /// Given user with ip 
        /// And ip location differs from residence country
        /// When IpMatch check is failed
        /// Then PoA collection step is requested
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldRequestPoA_WhenIpMatchCheckFailed(UsUserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            userInfo.IpAddress = IpAddresses.FailAddress;
            await _profileFixture.CreateAsync(userInfo);
            
            // Arrange
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var client = _publicApiClientFactory.Create(userId, userInfo.IpAddress);
            
            // Act
            await client.Applications.RegisterAsync();
            
            // Assert
            var @event = _eventsFixture.ShouldExistSingle<CollectionStepRequestedEvent>(e => e.EventArgs.UserId == userId && 
                                                                                             e.EventArgs.XPath == Documents.ProofOfAddress);
            var check = (await adminClient.Checks.GetAllAsync(userId)).First(c => c.Type == CheckType.IpMatch);
            using (new AssertionScope())
            {
                @event.EventArgs.IsRequired.Should().BeTrue();
                @event.EventArgs.IsReviewNeeded.Should().BeTrue();
                check.State.Should().Be(CheckState.Complete);
                check.Result.Should().Be(CheckResult.Failed);
                check.Decision.Should().BeNullOrEmpty();
            }
        }

        /// <summary>
        /// Scenario: Request W9Form step
        /// Given user with US tax residence  
        /// When TaxResidence check is failed
        /// Then W9Form collection step is requested
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldRequestW9Form_WhenTaxResidenceCheckFailed(NotUsUserInfo userInfo)
        {
            // Given
            var userId = userInfo.UserId;
            await _applicationFixture.BuildApplicationAsync(userInfo);
            
            // Arrange
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);
            var taxResidence = new TaxResidence { Countries = new[] { CountryCodes.Us } };
            
            // Act
            await _verificationDetailsFixture.UpdateByUserAsync(userId, builder => builder.With(taxResidence));
            
            // Assert
            var @event = _eventsFixture.ShouldExistSingle<CollectionStepRequestedEvent>(e => e.EventArgs.UserId == userId &&
                                                                                             e.EventArgs.XPath == Documents.W9Form);
            var check = (await client.Checks.GetAllAsync(userId)).First(c => c.Type == CheckType.TaxResidence);
            using (new AssertionScope())
            {
                @event.EventArgs.IsRequired.Should().BeTrue();
                @event.EventArgs.IsReviewNeeded.Should().BeTrue();
                check.State.Should().Be(CheckState.Complete);
                check.Result.Should().Be(CheckResult.Failed);
                check.Decision.Should().BeNullOrEmpty();
            }
        }

        /// <summary>
        /// Scenario: Request PoA step
        /// Given user with applied application
        /// When IdentityEnhanced check is failed
        /// Then PoA collection step is requested
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldRequestPoA_WhenIdentityEnhancedCheckFailed(GbUserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var applicantId = await _externalProfileFixture.GetApplicantIdAsync(userId);
            var checkOption = OnfidoCheckOption.Failed(CheckType.IdentityEnhanced);
            var checkOptions = OnfidoGroupedCheckOptions.Create(applicantId, checkOption);
            await OnfidoImposter.ConfigureCheckAsync(checkOptions);
            
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);
            var checks = await client.ExecuteUntilAsync(
                c => c.Checks.GetAllAsync(userId),
                cs => cs.FirstOrDefault(c => c.Type == CheckType.IdentityEnhanced) != null);
            var check = checks.First(check => check.Type == CheckType.IdentityEnhanced);

            // Act
            await _checksFixture.CompleteAsync(userId, check.Variant.Id, seed, null);
            
            // Assert
            var @event = _eventsFixture.ShouldExistSingle<CollectionStepRequestedEvent>(e => e.EventArgs.UserId == userId &&
                                                                                             e.EventArgs.XPath == Documents.ProofOfAddress);
            check = await client.Checks.GetAsync(userId, check.Id);
            using (new AssertionScope())
            {
                @event.EventArgs.IsRequired.Should().BeTrue();
                @event.EventArgs.IsReviewNeeded.Should().BeTrue();
                check.State.Should().Be(CheckState.Complete);
                check.Result.Should().Be(CheckResult.Failed);
                check.Decision.Should().BeNullOrEmpty();
            }
        }
        
        /// <summary>
        /// Scenario: Request PoI step
        /// Given user with applied application
        /// When IdentityDocument check is failed
        /// And check decision is "Resubmit"
        /// Then PoI step is requested
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldRequestPoi_WhenIdentityDocumentsCheckFailed(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var applicantId = await _externalProfileFixture.GetApplicantIdAsync(userId);
            var checkOption = OnfidoCheckOption.Failed(CheckType.IdentityDocument, CheckDecisions.Resubmit);
            var checkOptions = OnfidoGroupedCheckOptions.Create(applicantId, checkOption);
            await OnfidoImposter.ConfigureCheckAsync(checkOptions);
            
            // Arrange
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin); 

            // Act
            var checks = await client.ExecuteUntilAsync(
                c => c.Checks.GetAllAsync(userId),
                cs => cs.FirstOrDefault(c => c.Type == CheckType.IdentityDocument) != null);
            var check = checks.First(check => check.Type == CheckType.IdentityDocument);

            // Act
            await _checksFixture.CompleteAsync(userId, check.Variant.Id, seed, null);
            
            // Assert
            var @event = _eventsFixture.ShouldExistExact<CollectionStepRequestedEvent>(2, e => e.EventArgs.UserId == userId &&
                                                                                               e.EventArgs.XPath == Documents.ProofOfIdentity)[0];
            var poiStep = await client.CollectionStep.GetAsync(userId, @event.EventArgs.CollectionStepId);
            check = await client.Checks.GetAsync(userId, check.Id);
            using (new AssertionScope())
            {
                @event.EventArgs.IsRequired.Should().BeTrue();
                @event.EventArgs.IsReviewNeeded.Should().BeFalse();
                poiStep.State.Should().Be(CollectionStepState.Requested);
                check.State.Should().Be(CheckState.Complete);
                check.Result.Should().Be(CheckResult.Failed);
                check.Decision.Should().Be(CheckDecisions.Resubmit);
            }
        }
        
        /// <summary>
        /// Scenario: Request SelfiePhoto step
        /// Given user with applied application from GB
        /// When FacialSimilarity check is failed
        /// And check decision is "Resubmit"
        /// Then SelfiePhoto step is requested
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldRequestSelfiePhoto_WhenFacialSimilarityPhotoFailed(GbUserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var applicantId = await _externalProfileFixture.GetApplicantIdAsync(userId);
            var checkOption = OnfidoCheckOption.Failed(CheckType.FacialSimilarity, CheckDecisions.Resubmit);
            var checkOptions = OnfidoGroupedCheckOptions.Create(applicantId, checkOption);
            await OnfidoImposter.ConfigureCheckAsync(checkOptions);
            
            // Arrange
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);
            var checks = await client.ExecuteUntilAsync(
                c => c.Checks.GetAllAsync(userId),
                cs => cs.FirstOrDefault(c => c.Type == CheckType.FacialSimilarity) != null);
            var check = checks.First(check => check.Type == CheckType.FacialSimilarity);
            
            // Act
            await _checksFixture.CompleteAsync(userId, check.Variant.Id, seed, null);
            
            // Assert
            var @event = _eventsFixture.ShouldExistExact<CollectionStepRequestedEvent>(2, e => e.EventArgs.UserId == userId &&
                                                                                            e.EventArgs.XPath == Documents.SelfiePhoto)[0];
            var poiStep = await client.CollectionStep.GetAsync(userId, @event.EventArgs.CollectionStepId);
            check = await client.Checks.GetAsync(userId, check.Id);
            using (new AssertionScope())
            {
                @event.EventArgs.IsRequired.Should().BeTrue();
                @event.EventArgs.IsReviewNeeded.Should().BeFalse();
                poiStep.State.Should().Be(CollectionStepState.Requested);
                check.State.Should().Be(CheckState.Complete);
                check.Result.Should().Be(CheckResult.Failed);
                check.Decision.Should().Be(CheckDecisions.Resubmit);
            }
        }
        
        /// <summary>
        /// Scenario: Request SelfieVideo step
        /// Given user with applied application from GB
        /// When FaceDuplication check is failed
        /// And check decision is "Resubmit"
        /// Then SelfieVideo step is requested
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldRequestSelfieVideo_WhenFacialSimilarityVideoFailed(UsUserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var applicantId = await _externalProfileFixture.GetApplicantIdAsync(userId);
            var checkOption = OnfidoCheckOption.Failed(CheckType.FacialSimilarity, CheckDecisions.Resubmit);
            var checkOptions = OnfidoGroupedCheckOptions.Create(applicantId, checkOption);
            await OnfidoImposter.ConfigureCheckAsync(checkOptions);
            
            // Arrange
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);
            var checks = await client.ExecuteUntilAsync(
                c => c.Checks.GetAllAsync(userId),
                cs => cs.FirstOrDefault(c => c.Type == CheckType.FacialSimilarity) != null);
            var check = checks.First(check => check.Type == CheckType.FacialSimilarity);
            
            // Act
            await _checksFixture.CompleteAsync(userId, check.Variant.Id, seed, null);
            
            // Assert
            var @event = _eventsFixture.ShouldExistExact<CollectionStepRequestedEvent>(2, e => e.EventArgs.UserId == userId &&
                                                                                               e.EventArgs.XPath == Documents.SelfieVideo)[0];
            var poiStep = await client.CollectionStep.GetAsync(userId, @event.EventArgs.CollectionStepId);
            check = await client.Checks.GetAsync(userId, check.Id);
            using (new AssertionScope())
            {
                @event.EventArgs.IsRequired.Should().BeTrue();
                @event.EventArgs.IsReviewNeeded.Should().BeFalse();
                poiStep.State.Should().Be(CollectionStepState.Requested);
                check.State.Should().Be(CheckState.Complete);
                check.Result.Should().Be(CheckResult.Failed);
                check.Decision.Should().Be(CheckDecisions.Resubmit);
            }
        }
        
        /// <summary>
        /// Scenario: Request SelfiePhoto step
        /// Given user with applied application from GB
        /// When FaceDuplication check is failed
        /// And check decision is "Resubmit"
        /// Then SelfiePhoto step is requested
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldRequestSelfiePhoto_WhenFaceDuplicationPhotoFailed(GbUserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var applicantId = await _externalProfileFixture.GetApplicantIdAsync(userId);
            var checkOption = OnfidoCheckOption.Failed(CheckType.FaceDuplication, CheckDecisions.Resubmit);
            var checkOptions = OnfidoGroupedCheckOptions.Create(applicantId, checkOption);
            await OnfidoImposter.ConfigureCheckAsync(checkOptions);
            
            // Arrange
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);
            var checks = await client.ExecuteUntilAsync(
                c => c.Checks.GetAllAsync(userId),
                cs => cs.FirstOrDefault(c => c.Type == CheckType.FaceDuplication) != null);
            var check = checks.First(check => check.Type == CheckType.FaceDuplication);
            
            // Act
            await _checksFixture.CompleteAsync(userId, check.Variant.Id, seed, null);
            
            // Assert
            var @event = _eventsFixture.ShouldExistExact<CollectionStepRequestedEvent>(2, e => e.EventArgs.UserId == userId &&
                                                                                               e.EventArgs.XPath == Documents.SelfiePhoto)[0];
            var poiStep = await client.CollectionStep.GetAsync(userId, @event.EventArgs.CollectionStepId);
            check = await client.Checks.GetAsync(userId, check.Id);
            using (new AssertionScope())
            {
                @event.EventArgs.IsRequired.Should().BeTrue();
                @event.EventArgs.IsReviewNeeded.Should().BeFalse();
                poiStep.State.Should().Be(CollectionStepState.Requested);
                check.State.Should().Be(CheckState.Complete);
                check.Result.Should().Be(CheckResult.Failed);
                check.Decision.Should().Be(CheckDecisions.Resubmit);
            }
        }
        
        /// <summary>
        /// Scenario: Request SelfiePhoto step
        /// Given user with applied application from GB
        /// When FaceDuplication check is failed
        /// And check decision is "Resubmit"
        /// Then SelfieVideo step is requested
        /// And event is raised
        /// </summary>
        [Theory]
        public async Task ShouldRequestSelfieVideo_WhenFaceDuplicationVideoFailed(UsUserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);

            var applicantId = await _externalProfileFixture.GetApplicantIdAsync(userId);
            var checkOption = OnfidoCheckOption.Failed(CheckType.FaceDuplication, CheckDecisions.Resubmit);
            var checkOptions = OnfidoGroupedCheckOptions.Create(applicantId, checkOption);
            await OnfidoImposter.ConfigureCheckAsync(checkOptions);
            
            // Arrange
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var client = _adminApiClientFactory.Create(admin);
            var checks = await client.ExecuteUntilAsync(
                c => c.Checks.GetAllAsync(userId),
                cs => cs.FirstOrDefault(c => c.Type == CheckType.FaceDuplication) != null);
            var check = checks.First(check => check.Type == CheckType.FaceDuplication);
            
            // Act
            await _checksFixture.CompleteAsync(userId, check.Variant.Id, seed, null);
            
            // Assert
            var @event = _eventsFixture.ShouldExistExact<CollectionStepRequestedEvent>(2, e => e.EventArgs.UserId == userId &&
                                                                                               e.EventArgs.XPath == Documents.SelfieVideo)[0];
            var poiStep = await client.CollectionStep.GetAsync(userId, @event.EventArgs.CollectionStepId);
            check = await client.Checks.GetAsync(userId, check.Id);
            using (new AssertionScope())
            {
                @event.EventArgs.IsRequired.Should().BeTrue();
                @event.EventArgs.IsReviewNeeded.Should().BeFalse();
                poiStep.State.Should().Be(CollectionStepState.Requested);
                check.State.Should().Be(CheckState.Complete);
                check.Result.Should().Be(CheckResult.Failed);
                check.Decision.Should().Be(CheckDecisions.Resubmit);
            }
        }

        /// <summary>
        /// Scenario: Request PEP survey step
        /// Given PEP user 
        /// When RiskListsScreening check is failed
        /// Then PEP survey collection step is requested
        /// And event is raised
        /// </summary>
        [Theory, Ignore("No policy use this check variant")]
        public async Task ShouldRequestPepSurvey_WhenRiskListsScreeningCheckFailed(UserInfo userInfo, Seed seed)
        {
            // Given
            var userId = userInfo.UserId.Dump();
            await _applicationFixture.BuildApplicationAsync(userInfo);
            
            var profileId = await _externalProfileFixture.GetProfileIdAsync(userId, true, seed);
            var checkOptions = PassfortCheckOptions.Failed(isPep: true)
                                                   .WithProfileId(profileId);
            await PassfortImposter.ConfigureCheckAsync(checkOptions);
            
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);
            var checks = await adminClient.Checks.GetAllAsync(userId);
            var check = checks.First(check => check.Type == CheckType.RiskListsScreening);

            // Act
            await _checksFixture.CompleteAsync(userId, check.Variant.Id, seed, null);
            
            // Assert
            var @event = _eventsFixture.ShouldExistSingle<CollectionStepRequestedEvent>(e => e.EventArgs.UserId == userId &&
                                                                                             e.EventArgs.XPath == "Survey.CA6B7FB1-413D-449B-9038-32AB5B4914B6");
            @event.EventArgs.IsRequired.Should().BeTrue();
            @event.EventArgs.IsReviewNeeded.Should().BeTrue();
        }
    }
}
