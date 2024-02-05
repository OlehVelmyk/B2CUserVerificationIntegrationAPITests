using System;

namespace WX.B2C.User.Verification.Facade.Controllers.Internal.Mappers
{
    public interface IExternalProfileMapper
    {
        public Dtos.ExternalProfileDto Map(Core.Contracts.Dtos.Profile.ExternalProfileDto externalProfile);
    }

    internal class ExternalProfileMapper : IExternalProfileMapper
    {
        public Dtos.ExternalProfileDto Map(Core.Contracts.Dtos.Profile.ExternalProfileDto externalProfile)
        {
            if (externalProfile == null)
                throw new ArgumentNullException(nameof(externalProfile));

            var provider = externalProfile.Provider;

            return new Dtos.ExternalProfileDto
            {
                Name = provider,
                Id = externalProfile.Id
            };
        }
    }
}
