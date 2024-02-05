using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Optional;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Facade.Controllers.Public.Extensions;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Providers
{
    internal class VerificationPolicyProviderDecorator : IVerificationPolicyProvider
    {
        private const string SimplifiedPolicyHeader = "SimplifiedPolicy";

        private readonly IVerificationPolicyProvider _inner;
        private readonly IProfileService _profileService;
        private readonly IProfileStorage _profileStorage;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VerificationPolicyProviderDecorator(IVerificationPolicyProvider inner,
                                                   IProfileService profileService,
                                                   IProfileStorage profileStorage,
                                                   IHttpContextAccessor httpContextAccessor)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
            _profileStorage = profileStorage ?? throw new ArgumentNullException(nameof(profileStorage));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Guid> GetAsync(VerificationPolicySelectionContext selectionContext)
        {
            if (!TryGetPolicyId(out var policyId))
                return await _inner.GetAsync(selectionContext);

            var userId = _httpContextAccessor.HttpContext?.User?.GetUserId()
                ?? throw new ArgumentNullException($"{nameof(_httpContextAccessor.HttpContext)} can not be null.");

            var initiation = InitiationDto.CreateSystem("Set verified nationality to bypass verification.");
            var country = await _profileStorage.GetResidenceCountryAsync(userId);
            var patch = new VerificationDetailsPatch { Nationality = country.Some() };

            await _profileService.UpdateAsync(userId, patch, initiation);
            return policyId;
        }

        private bool TryGetPolicyId(out Guid policyId)
        {
            policyId = Guid.Empty;

            var headers = _httpContextAccessor.HttpContext?.Request?.Headers;
            if (headers == null)
                return false;

            return headers.TryGetValue(SimplifiedPolicyHeader, out var value) &&
                   Guid.TryParse(value, out policyId);
        }
    }
}
