using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Core.Contracts.Triggers;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Events.Internal.Events;
using WX.Messaging.Subscriber.HandlerResolving;

namespace WX.B2C.User.Verification.Facade.EventHandlers
{
    internal class TriggerCommandsEventHandler : BaseEventHandler,
                                                 IEventHandler<TriggerFiredEvent>
    {
        private readonly ITriggerVariantStorage _triggerVariantStorage;
        private readonly ITriggerCommandRunner _commandRunner;

        public TriggerCommandsEventHandler(
            ITriggerVariantStorage triggerVariantStorage,
            ITriggerCommandRunner commandRunner,
            EventHandlingContext context) : base(context)
        {
            _triggerVariantStorage = triggerVariantStorage ?? throw new ArgumentNullException(nameof(triggerVariantStorage));
            _commandRunner = commandRunner ?? throw new ArgumentNullException(nameof(commandRunner));
        }

        public Task HandleAsync(TriggerFiredEvent @event) =>
            Handle(@event, async args =>
            {
                var variantId = args.VariantId;
                var triggerVariant = await _triggerVariantStorage.GetAsync(variantId);

                var initiationDto = InitiationDto.CreateSystem(InitiationReasons.Trigger(variantId));
                await _commandRunner.RunAsync(args.UserId,
                                              args.ApplicationId,
                                              args.TriggerId,
                                              triggerVariant.Commands,
                                              initiationDto);
            });
    }
}