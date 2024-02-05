using System;
using System.Threading.Tasks;
using Optional;
using WX.B2C.Risks.Events;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Domain.Profile;
using WX.Messaging.Subscriber.HandlerResolving;

namespace WX.B2C.User.Verification.Facade.EventHandlers
{
    internal class RiskLevelEventHandler : BaseEventHandler,
                                           IEventHandler<RiskRatingChangedEvent>
    {
        private readonly IProfileService _profileService;

        public RiskLevelEventHandler(
            IProfileService profileService,
            EventHandlingContext context) : base(context)
        {
            _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
        }

        public Task HandleAsync(RiskRatingChangedEvent @event) =>
            Handle(@event, args =>
            {
                var userId = args.UserId;
                var riskLevel = (RiskLevel)args.RiskRating;
                var verificationDetailsPatch = new VerificationDetailsPatch
                {
                    RiskLevel = Option.Some<RiskLevel?>(riskLevel)
                };

                var initiation = InitiationDto.Create("WX.B2C.Risks", "Risk rating updated");
                return _profileService.UpdateAsync(userId, verificationDetailsPatch, initiation);
            });
    }
}