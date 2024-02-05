using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Module;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.PassFort.Client;
using WX.B2C.User.Verification.PassFort.Client.Models;
using WX.B2C.User.Verification.PassFort.Exceptions;
using WX.B2C.User.Verification.PassFort.Mappers;

namespace WX.B2C.User.Verification.PassFort
{
    // TODO: Inherit from BasePassFortGateway
    internal class PassFortProfileFactory : IExternalProfileFactory
    {
        private readonly IProfileStorage _profileStorage;
        private readonly IPassFortApiClientFactory _clientFactory;
        private readonly IIndividualDataMapper _mapper;

        public PassFortProfileFactory(IProfileStorage profileStorage,
                                      IPassFortApiClientFactory clientFactory, 
                                      IIndividualDataMapper mapper)
        {
            _profileStorage = profileStorage ?? throw new ArgumentNullException(nameof(profileStorage));
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ExternalProfileDto> CreateAsync(Guid userId)
        {
            var personalDetails = await _profileStorage.GetPersonalDetailsAsync(userId);
            if (personalDetails.ResidenceAddress is null)
                throw new PassFortProfileCreationPreconditionFailedException(userId);

            var address = await _mapper.MapAsync(personalDetails.ResidenceAddress);
            var client = _clientFactory.Create();
            var profile = new ProfileResource
            {
                Role = Constants.ProfileRoles.IndividualCustomer,
                CollectedData = new IndividualData
                {
                    EntityType = Constants.CollectedDataTypes.Individual,
                    PersonalDetails = new PersonalDetails
                    {
                        Dob = GetPassFortDateTime(personalDetails.DateOfBirth),
                        Name = new FullName
                        {
                            FamilyName = personalDetails.LastName,
                            GivenNames = new List<string> { personalDetails.FirstName }
                        }
                    },
                    ContactDetails = new IndividualDataContactDetails
                    {
                        Email = personalDetails.Email
                    },
                    AddressHistory = new List<DatedAddressHistoryItem>
                    {
                        new() { Address = address }
                    }
                }
            };

            var response = await client.Profiles.CreateAsync(profile);

            return new ExternalProfileDto { Id = response.Id, Provider = ExternalProviderType.PassFort };
        }

        private static string GetPassFortDateTime(DateTime? dateTime) => dateTime?.ToString("yyyy-MM-dd");
    }
}
