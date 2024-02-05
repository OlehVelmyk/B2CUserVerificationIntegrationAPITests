using System;
using System.Threading.Tasks;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Module;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Onfido.Client;
using WX.B2C.User.Verification.Onfido.Client.Models;

namespace WX.B2C.User.Verification.Onfido
{
    internal class OnfidoApplicantFactory : BaseOnfidoGateway, IExternalProfileFactory
    {
        private readonly IProfileStorage _profileStorage;
        private readonly IOnfidoApiClientFactory _clientFactory;

        public OnfidoApplicantFactory(IProfileStorage profileStorage, 
                                      IOnfidoApiClientFactory clientFactory,
                                      ILogger logger) : base(logger)
        {
            _profileStorage = profileStorage ?? throw new ArgumentNullException(nameof(profileStorage));
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        public async Task<ExternalProfileDto> CreateAsync(Guid userId)
        {
            var personalDetails = await _profileStorage.GetPersonalDetailsAsync(userId);

            var newApplicant = new NewApplicant
            {
                FirstName = personalDetails.FirstName,
                LastName = personalDetails.LastName,
                Email = personalDetails.Email
            };

            using var client = _clientFactory.Create();
            var applicantId = await HandleAsync(
                requestFactory: () => newApplicant,
                requestInvoker: client.Applicants.CreateAsync,
                responseMapper: applicant => applicant.Id);

            return new ExternalProfileDto
            {
                Id = applicantId,
                Provider = ExternalProviderType.Onfido
            };
        }
    }
}
