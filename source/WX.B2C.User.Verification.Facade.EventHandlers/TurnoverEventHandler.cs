using System;
using System.Threading.Tasks;
using Optional;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.Messaging.Subscriber.HandlerResolving;
using WX.Risk.Assessment.Transaction.Events.Events;

namespace WX.B2C.User.Verification.Facade.EventHandlers
{
    internal class TurnoverEventHandler : BaseEventHandler,
                                          IEventHandler<StandaloneRiskFactorUpdatedEvent>
    {
        private const string TurnoverRiskFactor = "AllTheTimeIncomeTurnoverRiskFactor";
        private readonly IProfileService _profileService;

        public TurnoverEventHandler(
            IProfileService profileService,
            EventHandlingContext context) : base(context)
        {
            _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
        }

        public Task HandleAsync(StandaloneRiskFactorUpdatedEvent @event) =>
            Handle(@event, args =>
            {
                var riskFactor = args.RiskFactor;
                if (riskFactor is not { Name: TurnoverRiskFactor })
                    return Task.CompletedTask;

                var userId = args.OwnerId;
                var verificationDetailsPatch = new VerificationDetailsPatch { Turnover = riskFactor.Weight.Some() };
                var initiationDto = InitiationDto.Create("WX.Risk.Assessment.Transaction", $"{nameof(StandaloneRiskFactorUpdatedEvent)} raised with {riskFactor.Name}");
                return _profileService.UpdateAsync(userId, verificationDetailsPatch, initiationDto);
            });
    }
}