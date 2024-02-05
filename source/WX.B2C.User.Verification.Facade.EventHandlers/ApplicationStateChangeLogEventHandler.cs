using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.Messaging.Subscriber.HandlerResolving;
using ApplicationState = WX.B2C.User.Verification.Events.Internal.Enums.ApplicationState;

namespace WX.B2C.User.Verification.Facade.EventHandlers
{
    internal class ApplicationStateChangelogEventHandler : BaseEventHandler, 
                                                           IEventHandler<ApplicationStateChangedEvent>
    {
        private readonly IApplicationStateChangelogRepository _applicationStateChangelogRepository;

        public ApplicationStateChangelogEventHandler(
            IApplicationStateChangelogRepository applicationStateChangelogRepository,
            EventHandlingContext context) : base(context)
        {
            _applicationStateChangelogRepository = applicationStateChangelogRepository ?? throw new ArgumentNullException(nameof(applicationStateChangelogRepository));
        }

        public Task HandleAsync(ApplicationStateChangedEvent @event) =>
            Handle(@event, args =>
            {
                if (args.NewState != ApplicationState.Approved)
                    return Task.CompletedTask;

                var applicationStateChangelog = new ApplicationStateChangelogDto
                {
                    ApplicationId = args.ApplicationId,
                    FirstApprovedDate = @event.CreatedOn,
                    LastApprovedDate = @event.CreatedOn
                };

                return _applicationStateChangelogRepository.SaveAsync(applicationStateChangelog);
            });
    }
}
