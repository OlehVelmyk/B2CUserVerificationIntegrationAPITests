using System;
using WX.B2C.User.Verification.Core.Contracts;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers
{
    public interface IExternalProfileMapper
    {
        public Dtos.ExternalProfileDto Map(Core.Contracts.Dtos.Profile.ExternalProfileDto externalProfile);
    }

    internal class ExternalProfileMapper : IExternalProfileMapper
    {
        private readonly IExternalLinkProvider _linkProvider;

        public ExternalProfileMapper(IExternalLinkProvider linkProvider)
        {
            _linkProvider = linkProvider ?? throw new ArgumentNullException(nameof(linkProvider));
        }

        public Dtos.ExternalProfileDto Map(Core.Contracts.Dtos.Profile.ExternalProfileDto externalProfile)
        {
            if (externalProfile == null)
                throw new ArgumentNullException(nameof(externalProfile));

            var provider = externalProfile.Provider;
            var link = _linkProvider.Get(externalProfile.Id, provider);

            return new Dtos.ExternalProfileDto
            {
                Name = provider,
                Link = link
            };
        }
    }
}
