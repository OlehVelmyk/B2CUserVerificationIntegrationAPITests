using System;
using System.Threading.Tasks;
using FluentValidation;
using WX.B2C.User.Profile.Events.Dtos;
using WX.B2C.User.Profile.Events.Events;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Facade.EventHandlers.Mappers;
using WX.Messaging.Subscriber.HandlerResolving;

namespace WX.B2C.User.Verification.Facade.EventHandlers
{
    internal class UserProfileEventHandler : BaseEventHandler,
                                             IEventHandler<UserProfileUpdatedEvent>,
                                             IEventHandler<UserProfileCreatedEvent>
    {
        private readonly IProfileService _profileService;
        private readonly IProfileDetailsMapper _personalDetailsMapper;
        private readonly IValidator<AddressDto> _addressValidator;

        public UserProfileEventHandler(
            IProfileService profileService,
            IProfileDetailsMapper personalDetailsMapper,
            IValidator<AddressDto> addressValidator,
            EventHandlingContext context) : base(context)
        {
            _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
            _personalDetailsMapper = personalDetailsMapper ?? throw new ArgumentNullException(nameof(personalDetailsMapper));
            _addressValidator = addressValidator ?? throw new ArgumentNullException(nameof(addressValidator));
        }

        public Task HandleAsync(UserProfileCreatedEvent @event) =>
            Handle(@event, args => UpdateProfileAsync(args.UserProfile, "Profile created"));

        /// <remarks>
        /// UserProfileCreatedEvent would not contain residence country
        /// therefore we should listen UserProfileUpdatedEvent
        /// and register verification application as soon as residence country appeared.
        /// However, it would be more intuitively to register verification application on UserProfileCreatedEvent.
        /// </remarks>
        public Task HandleAsync(UserProfileUpdatedEvent @event) =>
            Handle(@event, args => UpdateProfileAsync(args.UserProfile, "Profile updated"));

        private async Task UpdateProfileAsync(UserProfileDto userProfile, string reason)
        {
            await ValidateAddressAsync(userProfile);

            var patch = _personalDetailsMapper.Map(userProfile);
            var initiation = InitiationDto.Create("WX.B2C.Profile", reason);
            await _profileService.UpdateAsync(userProfile.UserId, patch, initiation);
        }

        private async Task ValidateAddressAsync(UserProfileDto userProfile)
        {
            if (userProfile.ResidenceAddress == null)
                return;

            var addressValidationResult = await _addressValidator.ValidateAsync(userProfile.ResidenceAddress);
            if (!addressValidationResult.IsValid)
                userProfile.ResidenceAddress = null;
        }
    }
}