using System;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Module;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.Domain.Events;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Core.Services
{
    internal class ExternalProfileProvider : IExternalProfileProvider
    {
        private readonly IExternalProfileRepository _externalProfileRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IIndex<ExternalProviderType, IExternalProfileFactory> _externalProfileFactories;

        public ExternalProfileProvider(
            IExternalProfileRepository externalProfileRepository,
            IEventPublisher eventPublisher,
            IIndex<ExternalProviderType, IExternalProfileFactory> externalProfileFactories)
        {
            _externalProfileRepository = externalProfileRepository ?? throw new ArgumentNullException(nameof(externalProfileRepository));
            _externalProfileFactories = externalProfileFactories ?? throw new ArgumentNullException(nameof(externalProfileFactories));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public async Task<ExternalProfileDto> GetOrCreateAsync(Guid userId, ExternalProviderType externalProviderType)
        {
            var externalProfile = await _externalProfileRepository.FindAsync(userId, externalProviderType);

            if (externalProfile != null)
                return externalProfile;

            externalProfile = await CreateExternalProfileAsync(userId, externalProviderType);
            await _externalProfileRepository.SaveAsync(userId, externalProfile);

            await _eventPublisher.PublishAsync(ExternalProfileCreated.Create(userId, externalProviderType));
            return externalProfile;
        }

        private Task<ExternalProfileDto> CreateExternalProfileAsync(Guid userId, ExternalProviderType externalProviderType) =>
            _externalProfileFactories.TryGetValue(externalProviderType, out var factory)
                ? factory.CreateAsync(userId)
                : throw new ArgumentOutOfRangeException(nameof(externalProviderType), externalProviderType,
                                                        "Cannot find factory to create external info in provider");
    }
}