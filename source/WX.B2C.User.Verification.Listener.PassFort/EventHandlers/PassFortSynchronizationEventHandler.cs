using System;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Listener.PassFort.Services;
using WX.Messaging.Subscriber.HandlerResolving;
using ExternalProviderType = WX.B2C.User.Verification.Events.Internal.Enums.ExternalProviderType;

namespace WX.B2C.User.Verification.Listener.PassFort.EventHandlers
{
    internal class PassFortSynchronizationEventHandler : BaseEventHandler,
                                                         IEventHandler<ApplicationStateChangedEvent>,
                                                         IEventHandler<TaskCompletedEvent>,
                                                         IEventHandler<TaskIncompleteEvent>,
                                                         IEventHandler<DocumentSubmittedEvent>,
                                                         IEventHandler<PersonalDetailsUpdatedEvent>,
                                                         IEventHandler<VerificationDetailsUpdatedEvent>,
                                                         IEventHandler<ExternalProfileCreatedEvent>,
                                                         IEventHandler<ApplicationAutomatedEvent>
    {
        private readonly IExternalProfileAdapter _externalProfileAdapter;
        private readonly IPassFortSynchronizationService _synchronizationService;

        public PassFortSynchronizationEventHandler(
            IExternalProfileAdapter externalProfileAdapter,
            IPassFortSynchronizationService synchronizationService,
            EventHandlingContext context) : base(context)
        {
            _externalProfileAdapter = externalProfileAdapter ?? throw new ArgumentNullException(nameof(externalProfileAdapter));
            _synchronizationService = synchronizationService ?? throw new ArgumentNullException(nameof(synchronizationService));
        }

        public Task HandleAsync(ApplicationAutomatedEvent @event) =>
            Handle(@event, args =>
            {
                return _externalProfileAdapter.ExecuteAsync(args.UserId, CreateNewApplicationAsync);

                Task CreateNewApplicationAsync(string profileId, string productId) =>
                    _synchronizationService.CreateNewApplicationAsync(profileId, productId, args.ApplicationId);
            });

        public Task HandleAsync(ApplicationStateChangedEvent @event) =>
            Handle(@event, args =>
            {
                var previousState = args.PreviousState.To<ApplicationState>();
                var newState = args.NewState.To<ApplicationState>();
                return _externalProfileAdapter.ExecuteAsync(args.UserId, UpdateApplicationState);

                Task UpdateApplicationState(string profileId, string productId) =>
                    _synchronizationService.UpdateApplicationStateAsync(profileId, productId, previousState, newState);
            });

        public Task HandleAsync(TaskCompletedEvent @event) =>
            Handle(@event, args =>
            {
                var taskType = args.Type.To<TaskType>();
                return _externalProfileAdapter.ExecuteAsync(args.UserId, UpdateTaskState);

                Task UpdateTaskState(string profileId) =>
                    _synchronizationService.UpdateTaskStateAsync(profileId, taskType, args.Result.To<TaskResult>());
            });

        public Task HandleAsync(TaskIncompleteEvent @event) =>
            Handle(@event, args =>
            {
                var taskType = args.Type.To<TaskType>();
                return _externalProfileAdapter.ExecuteAsync(args.UserId, UpdateTaskState);

                Task UpdateTaskState(string profileId) =>
                    _synchronizationService.UpdateTaskStateAsync(profileId, taskType, null);
            });

        public Task HandleAsync(DocumentSubmittedEvent @event) =>
            Handle(@event, args =>
            {
                return _externalProfileAdapter.ExecuteAsync(args.UserId, UploadDocument);

                Task UploadDocument(string profileId) =>
                    _synchronizationService.UploadDocumentAsync(profileId, args.DocumentId);
            });

        public Task HandleAsync(PersonalDetailsUpdatedEvent @event) =>
            Handle(@event, args =>
            {
                return _externalProfileAdapter.ExecuteAsync(args.UserId, UpdatePersonalDetails);

                Task UpdatePersonalDetails(string profileId) =>
                    _synchronizationService.UpdatePersonalDetailsAsync(profileId, args.Changes);
            });

        public Task HandleAsync(ExternalProfileCreatedEvent @event) =>
            Handle(@event, args =>
            {
                return args.ProviderType is ExternalProviderType.PassFort
                    ? _externalProfileAdapter.ExecuteAsync(args.UserId, UpdateNewProfile)
                    : Task.CompletedTask;

                Task UpdateNewProfile(string profileId) =>
                    _synchronizationService.UpdateNewProfileAsync(args.UserId, profileId);
            });

        public Task HandleAsync(VerificationDetailsUpdatedEvent @event) =>
            Handle(@event, args =>
            {
                var changes = args.Changes
                                  .Select(change => change.PropertyName)
                                  .ToArray();

                return _externalProfileAdapter.ExecuteAsync(args.UserId, UpdateVerificationDetails);

                Task UpdateVerificationDetails(string profileId) =>
                    _synchronizationService.UpdateVerificationDetailsAsync(args.UserId, profileId, changes);
            });
    }
}