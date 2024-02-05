using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Events.Internal.Enums;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.B2C.User.Verification.Listener.PassFort.Services;
using WX.Messaging.Subscriber.HandlerResolving;

namespace WX.B2C.User.Verification.Listener.PassFort.EventHandlers
{
    internal class PassFortTagsEventHandler : BaseEventHandler,
                                              IEventHandler<ApplicationStateChangedEvent>,
                                              IEventHandler<VerificationDetailsUpdatedEvent>,
                                              IEventHandler<PersonalDetailsUpdatedEvent>,
                                              IEventHandler<ExternalProfileCreatedEvent>
    {
        private readonly IPassFortTagService _tagsService;
        private readonly IExternalProfileAdapter _externalProfileAdapter;

        public PassFortTagsEventHandler(
            IPassFortTagService tagsService,
            IExternalProfileAdapter externalProfileAdapter,
            EventHandlingContext context) : base(context)
        {
            _tagsService = tagsService ?? throw new ArgumentNullException(nameof(tagsService));
            _externalProfileAdapter = externalProfileAdapter ?? throw new ArgumentNullException(nameof(externalProfileAdapter));
        }

        public Task HandleAsync(ExternalProfileCreatedEvent @event) =>
            Handle(@event, args => _externalProfileAdapter.ExecuteAsync(
                args.UserId,
                profileId => args.ProviderType switch
                {
                    ExternalProviderType.PassFort => _tagsService.CreateNewProfileTagsAsync(args.UserId, profileId),
                    ExternalProviderType.Onfido => _tagsService.SetOnfidoTagAsync(args.UserId, profileId),
                    _ => Task.CompletedTask
                }));

        public Task HandleAsync(ApplicationStateChangedEvent @event) =>
            Handle(@event, args =>
            {
                return args.NewState is ApplicationState.Cancelled
                    ? _externalProfileAdapter.ExecuteAsync(args.UserId, AddAccountClosureTag)
                    : Task.CompletedTask;

                Task AddAccountClosureTag(string profileId) =>
                    _tagsService.AddAccountClosureTagAsync(profileId);
            });

        public Task HandleAsync(VerificationDetailsUpdatedEvent @event) =>
            Handle(@event, args => _externalProfileAdapter.ExecuteAsync(
                args.UserId,
                profileId => _tagsService.UpdateVerificationDetailsTagsAsync(profileId, args.Changes)));

        public Task HandleAsync(PersonalDetailsUpdatedEvent @event) =>
            Handle(@event, args => _externalProfileAdapter.ExecuteAsync(
                args.UserId,
                profileId => _tagsService.UpdatePersonalDetailsTagsAsync(profileId, args.Changes)));
    }
}