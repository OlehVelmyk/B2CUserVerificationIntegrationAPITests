using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.Core.Services.Mappers;
using WX.B2C.User.Verification.Domain.Events;
using WX.B2C.User.Verification.Domain.Shared;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Core.Services
{
    internal class ProfileService : IProfileService
    {
        private readonly IPersonalDetailsRepository _personalDetailsRepository;
        private readonly IVerificationDetailsRepository _verificationDetailsRepository;
        private readonly IExternalProfileRepository _externalProfileRepository;
        private readonly IProfilePatcher _profilePatcher;
        private readonly IInitiationMapper _initiationMapper;
        private readonly IEventPublisher _eventPublisher;

        public ProfileService(IPersonalDetailsRepository personalDetailsRepository,
                              IVerificationDetailsRepository verificationDetailsRepository,
                              IExternalProfileRepository externalProfileRepository,
                              IProfilePatcher profilePatcher,
                              IInitiationMapper initiationMapper,
                              IEventPublisher eventPublisher)
        {
            _personalDetailsRepository = personalDetailsRepository ?? throw new ArgumentNullException(nameof(personalDetailsRepository));
            _verificationDetailsRepository = verificationDetailsRepository ?? throw new ArgumentNullException(nameof(verificationDetailsRepository));
            _externalProfileRepository = externalProfileRepository ?? throw new ArgumentNullException(nameof(externalProfileRepository));
            _profilePatcher = profilePatcher ?? throw new ArgumentNullException(nameof(profilePatcher));
            _initiationMapper = initiationMapper ?? throw new ArgumentNullException(nameof(initiationMapper));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public async Task UpdateAsync(Guid userId, PersonalDetailsPatch patch, InitiationDto initiationDto)
        {
            var model = await _personalDetailsRepository.FindAsync(userId) ?? new PersonalDetailsDto { UserId = userId };

            // TODO Open question if we should clear value if it is cleared in profile. For now we rely on values in b2c.profile
            var changes = _profilePatcher.ApplyPatch(model, patch);
            if (changes.IsNullOrEmpty())
                return;

            await _personalDetailsRepository.SaveAsync(model);
            var @event = PersonalDetailsUpdated.Create(userId, changes, _initiationMapper.Map(initiationDto));
            await _eventPublisher.PublishAsync(@event);
        }

        public async Task UpdateAsync(Guid userId, VerificationDetailsPatch patch, InitiationDto initiationDto)
        {
            var model = await _verificationDetailsRepository.FindAsync(userId) ?? new VerificationDetailsDto { UserId = userId };

            var changes = _profilePatcher.ApplyPatch(model, patch);
            if (changes.IsNullOrEmpty())
                return;

            await _verificationDetailsRepository.SaveAsync(model);
            var @event = VerificationDetailsUpdated.Create(userId, changes, _initiationMapper.Map(initiationDto));
            await _eventPublisher.PublishAsync(@event);
        }

        public Task UpdateAsync(Guid userId, ExternalProfileDto externalProfileDto)
        {
            return _externalProfileRepository.SaveAsync(userId, externalProfileDto);
        }
    }
}
