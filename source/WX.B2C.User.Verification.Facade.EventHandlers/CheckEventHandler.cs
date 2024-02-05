using System;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Automation;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Events.Internal.Enums;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.Messaging.Subscriber.HandlerResolving;
using CheckResult = WX.B2C.User.Verification.Events.Internal.Enums.CheckResult;

namespace WX.B2C.User.Verification.Facade.EventHandlers
{
    internal class CheckEventHandler : BaseEventHandler,
                                       IEventHandler<CheckCreatedEvent>,
                                       IEventHandler<CheckPerformedEvent>,
                                       IEventHandler<CheckCompletedEvent>,
                                       IEventHandler<CollectionStepRequestedEvent>,
                                       IEventHandler<CollectionStepCompletedEvent>,
                                       IEventHandler<PersonalDetailsUpdatedEvent>,
                                       IEventHandler<VerificationDetailsUpdatedEvent>,
                                       IEventHandler<ApplicationAutomatedEvent>,
                                       IEventHandler<ChecksCreatedEvent>,
                                       IEventHandler<TaskCompletedEvent>
    {
        private static readonly string[] ObservedProperties =
        {
            XPathes.IdDocumentNumber.ToString(),
            XPathes.VerifiedNationality.ToString()
        };

        private readonly ICheckEventObserver _checkEventObserver;
        private readonly IApplicationStorage _applicationStorage;

        public CheckEventHandler(
            ICheckEventObserver checkEventObserver,
            IApplicationStorage applicationStorage,
            EventHandlingContext context) : base(context)
        {
            _checkEventObserver = checkEventObserver ?? throw new ArgumentNullException(nameof(checkEventObserver));
            _applicationStorage = applicationStorage ?? throw new ArgumentNullException(nameof(applicationStorage));
        }

        public Task HandleAsync(PersonalDetailsUpdatedEvent @event) =>
            Handle(@event, args =>
            {
                var changedProperties = args.Changes
                                            .Select(property => property.PropertyName)
                                            .Except(new string[] { XPathes.ResidenceAddress })
                                            .ToArray();
                return _checkEventObserver.OnProfileChanged(args.UserId, changedProperties);
            });

        public Task HandleAsync(VerificationDetailsUpdatedEvent @event) =>
            Handle(@event, args =>
            {
                var initiator = args.Initiation.Initiator;
                var changedProperties = args.Changes
                                            .Where(property => !property.IsReset)
                                            .Select(property => property.PropertyName)
                                            .Where(ObservedProperties.Contains)
                                            .ToArray();

                if (initiator != Initiators.System || !changedProperties.Any()) return Task.CompletedTask;

                // Re-instruct dependent checks if any verification details were changed by system.
                return _checkEventObserver.OnProfileChanged(args.UserId, changedProperties);
            });

        public Task HandleAsync(CollectionStepRequestedEvent @event) =>
            Handle(@event, args =>
            {
                // Re-instruct dependent checks if admin requested collection step

                var initiator = args.Initiation.Initiator;
                return initiator.Contains(Initiators.Admin)
                    ? _checkEventObserver.OnCollectionStepRequested(args.UserId, args.XPath)
                    : Task.CompletedTask;
            });

        public Task HandleAsync(CollectionStepCompletedEvent @event) =>
            Handle(@event, async args =>
            {
                if (args.ReviewResult is CollectionStepReviewResult.Rejected)
                    return;

                if (!await IsAutomatedAsync(args.UserId))
                    return;

                await _checkEventObserver.OnCollectionStepCompleted(args.UserId, args.XPath);
            });

        public Task HandleAsync(CheckCreatedEvent @event) =>
            Handle(@event, async args =>
            {
                // TODO: Remove when automating will be enabled for all users
                var specialCheckVariants = new[]
                {
                    Guid.Parse("29AAC87B-3AD4-40E0-B34F-3685CA64805D"),
                    Guid.Parse("23714F13-CBF6-41A4-85C6-719991E6C3F3")
                };
                if (!await IsAutomatedAsync(args.UserId) && !specialCheckVariants.Contains(args.VariantId))
                    return;

                await _checkEventObserver.OnCheckCreated(args.UserId, args.CheckId);
            });

        public Task HandleAsync(ChecksCreatedEvent @event) =>
            Handle(@event, async args =>
            {
                if (!await IsAutomatedAsync(args.UserId))
                    return;

                await _checkEventObserver.OnChecksCreated(args.UserId, args.Checks);
            });

        public Task HandleAsync(CheckPerformedEvent @event) =>
            Handle(@event, args => _checkEventObserver.OnCheckPerformed(args.CheckId));

        public Task HandleAsync(CheckCompletedEvent @event) =>
            Handle(@event, args => args.Result switch
            {
                CheckResult.Passed => _checkEventObserver.OnCheckPassed(args.CheckId),
                CheckResult.Failed => _checkEventObserver.OnCheckFailed(args.CheckId),
                _ => throw new ArgumentOutOfRangeException(nameof(args.Result), args.Result, "Unsupported check result.")
            });

        public Task HandleAsync(TaskCompletedEvent @event) =>
            Handle(@event, args => _checkEventObserver.OnTaskCompleted(args.UserId));

        /// <summary>
        /// TODO WRXB-10546 remove in phase 2 when all users finally migrated
        /// </summary>
        public Task HandleAsync(ApplicationAutomatedEvent @event) =>
            Handle(@event, args => _checkEventObserver.OnApplicationAutomatedAsync(args.UserId));

        /// <summary>
        /// TODO WRXB-10546 remove in phase 2 when all users finally migrated
        /// </summary>
        private Task<bool> IsAutomatedAsync(Guid userId) =>
            _applicationStorage.IsAutomatedAsync(userId, ProductType.WirexBasic);
    }
}
