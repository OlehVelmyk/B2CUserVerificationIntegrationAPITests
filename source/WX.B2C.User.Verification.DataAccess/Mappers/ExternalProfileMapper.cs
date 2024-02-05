using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.DataAccess.Entities;

namespace WX.B2C.User.Verification.DataAccess.Mappers
{
    internal interface IExternalProfileMapper
    {
        ExternalProfileDto Map(ExternalProfile info);

        ExternalProfile Map(Guid userId, ExternalProfileDto externalProfileDto);

        void Update(ExternalProfileDto source, ExternalProfile target);
    }

    internal class ExternalProfileMapper : IExternalProfileMapper
    {
        public ExternalProfileDto Map(ExternalProfile info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            return new ExternalProfileDto
            {
                Provider = info.Provider,
                Id = info.ExternalId
            };
        }

        public ExternalProfile Map(Guid userId, ExternalProfileDto externalProfileDto)
        {
            if (externalProfileDto == null)
                throw new ArgumentNullException(nameof(externalProfileDto));

            var entity = new ExternalProfile
            {
                UserId = userId,
                Provider = externalProfileDto.Provider
            };
            Update(externalProfileDto, entity);
            return entity;
        }

        public void Update(ExternalProfileDto source, ExternalProfile target)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.ExternalId = source.Id;
        }
    }
}