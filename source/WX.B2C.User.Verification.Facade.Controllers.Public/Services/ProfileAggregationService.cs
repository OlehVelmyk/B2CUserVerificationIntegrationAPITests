using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Facade.Controllers.Public.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Public.Mappers;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Services
{
    public interface IProfileAggregationService
    {
        Task<ProfileDto> AggregateAsync(Guid userId);
    }

    internal class ProfileAggregationService : IProfileAggregationService
    {
        private readonly IProfileStorage _profileStorage;
        private readonly IApplicationStorage _applicationStorage;
        private readonly IVerificationDetailsMapper _verificationDetailsMapper;

        public ProfileAggregationService(
            IProfileStorage profileStorage,
            IVerificationDetailsMapper verificationDetailsMapper, IApplicationStorage applicationStorage)
        {
            _profileStorage = profileStorage ?? throw new ArgumentNullException(nameof(profileStorage));
            _applicationStorage = applicationStorage ?? throw new ArgumentNullException(nameof(applicationStorage));
            _verificationDetailsMapper = verificationDetailsMapper ?? throw new ArgumentNullException(nameof(verificationDetailsMapper));
        }

        public async Task<ProfileDto> AggregateAsync(Guid userId)
        {
            var verificationDetails = await _profileStorage.FindVerificationDetailsAsync(userId);
            var applicationState = await _applicationStorage.FindStateAsync(userId, ProductType.WirexBasic);

            return new ProfileDto
            {
                ApplicationState = applicationState,
                VerificationDetails = verificationDetails == null
                    ? null
                    : _verificationDetailsMapper.Map(verificationDetails)
            };
        }
    }
}