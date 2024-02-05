using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Services.Mappers;
using WX.B2C.User.Verification.Core.Services.Utilities;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Core.Services
{
    internal class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IInitiationMapper _initiationMapper;

        public ApplicationService(
            IApplicationRepository applicationRepository,
            ITaskRepository taskRepository,
            IInitiationMapper initiationMapper,
            IEventPublisher eventPublisher)
        {
            _applicationRepository = applicationRepository ?? throw new ArgumentNullException(nameof(applicationRepository));
            _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
            _initiationMapper = initiationMapper ?? throw new ArgumentNullException(nameof(initiationMapper));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public Task RegisterAsync(Guid userId, NewVerificationApplicationDto newApplication, InitiationDto initiationDto) =>
            CreateAsync(async () =>
            {
                var initiation = _initiationMapper.Map(initiationDto);
                var application = await _applicationRepository.FindAsync(userId, newApplication.ProductType);
                return application ?? Application.Create(userId, newApplication.PolicyId, newApplication.ProductType, initiation);
            });

        public Task AddRequiredTasksAsync(Guid applicationId, Guid[] taskIds, InitiationDto initiationDto) =>
            UpdateAsync(applicationId, async application =>
            {
                foreach (var taskId in taskIds)
                {
                    var task = await _taskRepository.GetAsync(taskId);
                    var initiation = _initiationMapper.Map(initiationDto);
                    application.AddRequiredTask(task, initiation);
                }
            });

        public Task ApproveAsync(Guid applicationId, InitiationDto initiationDto) =>
            UpdateAsync(applicationId, application =>
            {
                application.Approve(_initiationMapper.Map(initiationDto));
            });

        public Task RejectAsync(Guid applicationId, InitiationDto initiationDto) =>
            UpdateAsync(applicationId, application =>
            {
                application.Reject(_initiationMapper.Map(initiationDto));
            });

        public Task RequestReviewAsync(Guid applicationId, InitiationDto initiationDto) =>
            UpdateAsync(applicationId, application =>
            {
                application.RequestReview(_initiationMapper.Map(initiationDto));
            });

        public Task RevertDecisionAsync(Guid applicationId, InitiationDto initiationDto) =>
            UpdateAsync(applicationId, application =>
            {
                application.RevertDecision(_initiationMapper.Map(initiationDto));
            });

        public Task AutomateAsync(Guid applicationId, InitiationDto initiationDto) =>
            UpdateAsync(applicationId, application =>
            {
                application.Automate(_initiationMapper.Map(initiationDto));
            });
    
        private Task<Application> CreateAsync(Func<Task<Application>> create)
        {
            if (create == null)
                throw new ArgumentNullException(nameof(create));
            
            return AppCore.ApplyChangesAsync(create, SaveAndPublishAsync);
        }

        private Task<Application> UpdateAsync(Guid applicationId, Action<Application> update)
        {
            if (update == null)
                throw new ArgumentNullException(nameof(update));

            return AppCore.ApplyChangesAsync(
                () => _applicationRepository.GetAsync(applicationId),
                update,
                SaveAndPublishAsync);
        }

        private Task<Application> UpdateAsync(Guid applicationId, Func<Application, Task> update)
        {
            if (update == null)
                throw new ArgumentNullException(nameof(update));

            return AppCore.ApplyChangesAsync(
            () => _applicationRepository.GetAsync(applicationId),
            update,
            SaveAndPublishAsync);
        }

        private async Task SaveAndPublishAsync(Application application)
        {
            await _applicationRepository.SaveAsync(application);
            await _eventPublisher.PublishAsync(application.CommitEvents());
        }
    }
}
