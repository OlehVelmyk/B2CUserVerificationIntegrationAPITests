using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WX.B2C.Risks.Events.Enums;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Public.Client;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Fixtures.Events;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Events.Events;
using WX.B2C.User.Verification.Events.Internal.Enums;
using WX.B2C.User.Verification.Events.Internal.Extensions;
using WX.B2C.User.Verification.Extensions;
using AdminApi = WX.B2C.User.Verification.Api.Admin.Client.Models;
using InternalEvents = WX.B2C.User.Verification.Events.Internal.Events;


namespace WX.B2C.User.Verification.Component.Tests.Fixtures
{
    internal class ApplicationFixture
    {
        private static readonly IReadOnlyDictionary<Type, TaskType[]> OnboardingTasksMap = CreateOnboardingTasksMap();
        private static readonly IReadOnlyDictionary<Type, CheckType[]> AcceptanceChecksMap = CreateAcceptanceChecksMap();

        private readonly TaskFixture _taskFixture;
        private readonly ProfileFixture _profileFixture;
        private readonly UserRiskLevelFixture _userRiskLevelFixture;
        private readonly EventsFixture _eventsFixture;
        private readonly PublicApiClientFactory _publicApiClientFactory;
        private readonly AdminApiClientFactory _adminApiClientFactory;
        private readonly AdministratorFactory _adminFactory;

        public ApplicationFixture(TaskFixture taskFixture,
                                  ProfileFixture profileFixture,
                                  UserRiskLevelFixture userRiskLevelFixture,
                                  EventsFixture eventsFixture,
                                  PublicApiClientFactory apiClientFactory,
                                  AdminApiClientFactory adminApiClientFactory,
                                  AdministratorFactory adminFactory)
        {
            _taskFixture = taskFixture ?? throw new ArgumentNullException(nameof(taskFixture));
            _profileFixture = profileFixture ?? throw new ArgumentNullException(nameof(profileFixture));
            _userRiskLevelFixture = userRiskLevelFixture ?? throw new ArgumentNullException(nameof(userRiskLevelFixture));
            _eventsFixture = eventsFixture ?? throw new ArgumentNullException(nameof(eventsFixture));
            _publicApiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
            _adminApiClientFactory = adminApiClientFactory ?? throw new ArgumentNullException(nameof(adminApiClientFactory));
            _adminFactory = adminFactory ?? throw new ArgumentNullException(nameof(adminFactory));
        }

        public static TaskType[] GetOnboardingTasks(UserInfo userInfo) => OnboardingTasksMap[userInfo.GetType()];

        public static CheckType[] GetAcceptanceChecks(UserInfo userInfo) => AcceptanceChecksMap[userInfo.GetType()];

        public async Task RegisterApplicationAsync(UserInfo userInfo)
        {
            await _profileFixture.CreateAsync(userInfo);

            var correlationId = Guid.NewGuid();
            var client = _publicApiClientFactory.Create(userInfo.UserId, userInfo.IpAddress, correlationId);
            await client.Applications.RegisterAsync();

            _eventsFixture.ShouldExistSingle<ApplicationRegisteredEvent>(correlationId);
        }

        public async Task BuildApplicationAsync(UserInfo userInfo)
        {
            await _profileFixture.CreateAsync(userInfo);

            var correlationId = Guid.NewGuid();
            var client = _publicApiClientFactory.Create(userInfo.UserId, userInfo.IpAddress, correlationId);
            await client.Applications.RegisterAsync();

            ShouldBuildApplication(correlationId);
        }

        public async Task ApproveAsync(Guid userId, Guid applicationId, Seed seed)
        {
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            var application = await adminClient.Applications.GetAsync(userId, applicationId);
            if (application.State is AdminApi.ApplicationState.Approved)
                return;

            var previousEventCorrelationIds = _eventsFixture
                .GetAllEvents<InternalEvents.ApplicationStateChangedEvent>(IsApproved)
                .Select(e => e.CorrelationId)
                .ToArray();

            var taskIds = application.RequiredTasks.Select(task => task.Id).ToArray();
            await CompeteTasksAsync(userId, taskIds, seed);
            await _userRiskLevelFixture.ChangeRiskLevelAsync(RiskRating.Low, userId);

            _eventsFixture.ShouldExistSingle<InternalEvents.ApplicationStateChangedEvent>(
                e => IsApproved(e) && !e.CorrelationId.In(previousEventCorrelationIds));

            bool IsApproved(InternalEvents.ApplicationStateChangedEvent e) =>
                e.EventArgs.ApplicationId == applicationId && e.EventArgs.NewState is ApplicationState.Approved;
        }

        public async Task CancelAsync(Guid userId, Guid applicationId, [CallerMemberName] string callerMethod = null)
        {
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            var application = await adminClient.Applications.GetAsync(userId, applicationId);
            if (application.State is AdminApi.ApplicationState.Cancelled)
                return;
            if (application.State is not AdminApi.ApplicationState.Approved)
                throw new InvalidOperationException();

            var reason = new AdminApi.ReasonDto(callerMethod);
            await adminClient.Applications.RejectAsync(reason, userId, applicationId);

            _eventsFixture.ShouldExistSingle<InternalEvents.ApplicationStateChangedEvent>(
                e => e.CorrelationId == adminClient.CorrelationId && e.EventArgs.NewState is ApplicationState.Cancelled);
        }

        public async Task RejectAsync(Guid userId, Guid applicationId, [CallerMemberName] string callerMethod = null)
        {
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            var application = await adminClient.Applications.GetAsync(userId, applicationId);
            if (application.State is AdminApi.ApplicationState.Rejected)
                return;
            if (application.State is not AdminApi.ApplicationState.Applied)
                throw new InvalidOperationException();

            var reason = new AdminApi.ReasonDto(callerMethod);
            await adminClient.Applications.RejectAsync(reason, userId, applicationId);

            _eventsFixture.ShouldExistSingle<InternalEvents.ApplicationStateChangedEvent>(
                e => e.CorrelationId == adminClient.CorrelationId && e.EventArgs.NewState is ApplicationState.Rejected);
        }

        public async Task RequestReviewAsync(Guid userId, Guid applicationId, [CallerMemberName] string callerMethod = null)
        {
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            var application = await adminClient.Applications.GetAsync(userId, applicationId);
            if (application.State is AdminApi.ApplicationState.InReview)
                return;
            if (application.State is not AdminApi.ApplicationState.Approved)
                throw new InvalidOperationException();

            var reason = new AdminApi.ReasonDto(callerMethod);
            await adminClient.Applications.RequestReviewAsync(reason, userId, applicationId);

            _eventsFixture.ShouldExistSingle<InternalEvents.ApplicationStateChangedEvent>(
                e => e.EventArgs.ApplicationId == applicationId && e.EventArgs.NewState is ApplicationState.InReview);
        }

        public async Task CompeteTasksAsync(Guid userId, Guid[] taskIds, Seed seed)
        {
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            var tasks = (await adminClient.Tasks.GetAllAsync(userId))
                .Where(task => task.Id.In(taskIds));

            var orderedTasks = OrderTasks(tasks);
            await orderedTasks.ForeachConsistently(task => _taskFixture.CompleteAsync(userId, task.Id, seed));
        }

        private IEnumerable<AdminApi.TaskDto> OrderTasks(IEnumerable<AdminApi.TaskDto> tasks)
        {
            var duplicationScreeningTask = tasks.FirstOrDefault(t => t.Type is AdminApi.TaskType.DuplicationScreening);
            var identityTask = tasks.FirstOrDefault(t => t.Type is AdminApi.TaskType.Identity);

            var map = tasks.GroupBy(t => t.Priority).ToDictionary(g => g.Key, g => g.ToList());
            var item = map.FirstOrDefault(p => p.Value.Contains(duplicationScreeningTask));

            var group = item.Value;
            if (group is not null)
            {
                group.Remove(identityTask);
                group.Insert(0, identityTask);
                group.Remove(duplicationScreeningTask);
                group.Add(duplicationScreeningTask);
            }

            return map.OrderBy(g => g.Key).SelectMany(g => g.Value);
        }

        private void ShouldBuildApplication(Guid correlationId)
        {
            _eventsFixture.ShouldExistSingle<InternalEvents.VerificationDetailsUpdatedEvent>(
                e => e.CorrelationId == correlationId && e.EventArgs.Changes.Find<string>(VerificationDetails.IpAddress) is not null);

            _eventsFixture.ShouldExistSingle<ApplicationRegisteredEvent>(correlationId);
            _eventsFixture.ShouldExist<InternalEvents.CollectionStepRequestedEvent>(correlationId);
            _eventsFixture.ShouldExist<InternalEvents.TaskCreatedEvent>(correlationId);
            _eventsFixture.ShouldExist<InternalEvents.CheckCreatedEvent>(correlationId);
            _eventsFixture.ShouldExist<InternalEvents.ApplicationRequiredTaskAddedEvent>(correlationId);
            _eventsFixture.ShouldExistSingle<InternalEvents.ApplicationAutomatedEvent>(correlationId);
        }

        private static IReadOnlyDictionary<Type, TaskType[]> CreateOnboardingTasksMap() =>
            new Dictionary<Type, TaskType[]>
            {
                [typeof(GbUserInfo)] = new[]
                {
                    TaskType.Identity, TaskType.Address, TaskType.TaxResidence, TaskType.DuplicationScreening, TaskType.RiskListsScreening
                },
                [typeof(EeaUserInfo)] = new[]
                {
                    TaskType.Identity, TaskType.Address, TaskType.TaxResidence, TaskType.DuplicationScreening, TaskType.RiskListsScreening
                },
                [typeof(UsUserInfo)] = new[]
                {
                    TaskType.Identity, TaskType.Address, TaskType.TaxResidence, TaskType.DuplicationScreening, TaskType.RiskListsScreening,
                    TaskType.FraudScreening, TaskType.FinancialCondition
                },
                [typeof(ApacUserInfo)] = new[]
                {
                    TaskType.Identity, TaskType.Address, TaskType.TaxResidence, TaskType.DuplicationScreening, TaskType.RiskListsScreening
                },
                [typeof(RoWUserInfo)] = new[]
                {
                    TaskType.Identity, TaskType.Address, TaskType.TaxResidence, TaskType.DuplicationScreening, TaskType.RiskListsScreening
                },
                [typeof(GlobalUserInfo)] = new[]
                {
                    TaskType.Identity, TaskType.Address, TaskType.TaxResidence, TaskType.DuplicationScreening, TaskType.RiskListsScreening
                },
                [typeof(RuUserInfo)] = new[]
                {
                    TaskType.Identity, TaskType.Address, TaskType.TaxResidence, TaskType.DuplicationScreening, TaskType.RiskListsScreening
                }
            };

        private static IReadOnlyDictionary<Type, CheckType[]> CreateAcceptanceChecksMap() =>
            new Dictionary<Type, CheckType[]>
            {
                [typeof(GbUserInfo)] = new[]
                {
                    CheckType.IdentityDocument, CheckType.IdentityEnhanced, CheckType.FacialSimilarity, CheckType.FaceDuplication, CheckType.IpMatch, 
                    CheckType.TaxResidence, CheckType.IdDocNumberDuplication, CheckType.NameAndDoBDuplication, CheckType.RiskListsScreening
                },
                [typeof(EeaUserInfo)] = new[]
                {
                    CheckType.IdentityDocument, CheckType.FacialSimilarity, CheckType.FaceDuplication, CheckType.IpMatch,
                    CheckType.TaxResidence, CheckType.IdDocNumberDuplication, CheckType.NameAndDoBDuplication, CheckType.RiskListsScreening
                },
                [typeof(UsUserInfo)] = new[]
                {
                    CheckType.IdentityDocument, CheckType.FacialSimilarity, CheckType.FaceDuplication, CheckType.IpMatch,
                    CheckType.IdDocNumberDuplication, CheckType.NameAndDoBDuplication, CheckType.FraudScreening, CheckType.RiskListsScreening
                },
                [typeof(ApacUserInfo)] = new[]
                {
                    CheckType.IdentityDocument, CheckType.FacialSimilarity, CheckType.FaceDuplication, CheckType.IpMatch,
                    CheckType.TaxResidence, CheckType.IdDocNumberDuplication, CheckType.NameAndDoBDuplication, CheckType.RiskListsScreening
                },
                [typeof(RoWUserInfo)] = new[]
                {
                    CheckType.IdentityDocument, CheckType.FacialSimilarity, CheckType.FaceDuplication, CheckType.IpMatch,
                    CheckType.TaxResidence, CheckType.IdDocNumberDuplication, CheckType.NameAndDoBDuplication, CheckType.RiskListsScreening
                },
                [typeof(GlobalUserInfo)] = new[]
                {
                    CheckType.IdentityDocument, CheckType.FacialSimilarity, CheckType.FaceDuplication, CheckType.TaxResidence,
                    CheckType.IdDocNumberDuplication, CheckType.NameAndDoBDuplication, CheckType.RiskListsScreening
                },
                [typeof(RuUserInfo)] = new[]
                {
                    CheckType.IdentityDocument, CheckType.FacialSimilarity, CheckType.FaceDuplication, CheckType.IpMatch,
                    CheckType.TaxResidence, CheckType.IdDocNumberDuplication, CheckType.NameAndDoBDuplication, CheckType.RiskListsScreening
                }
            };
    }
}
