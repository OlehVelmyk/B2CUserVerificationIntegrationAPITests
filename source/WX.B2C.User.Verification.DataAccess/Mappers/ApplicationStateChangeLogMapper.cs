using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.DataAccess.Mappers
{
    internal interface IApplicationStateChangelogMapper
    {
        ApplicationStateChangelogDto Map(Entities.ApplicationStateChangelog entity);

        Entities.ApplicationStateChangelog MapToEntity(ApplicationStateChangelogDto entity);

        void Update(Entities.ApplicationStateChangelog source, ApplicationStateChangelogDto target);
    }

    internal class ApplicationStateChangelogMapper : IApplicationStateChangelogMapper
    {
        public ApplicationStateChangelogDto Map(Entities.ApplicationStateChangelog entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new ApplicationStateChangelogDto
            {
                ApplicationId = entity.ApplicationId,
                FirstApprovedDate = entity.FirstApprovedDate,
                LastApprovedDate = entity.LastApprovedDate
            };
        }

        public Entities.ApplicationStateChangelog MapToEntity(ApplicationStateChangelogDto entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new Entities.ApplicationStateChangelog
            {
                ApplicationId = entity.ApplicationId,
                FirstApprovedDate = entity.FirstApprovedDate,
                LastApprovedDate = entity.LastApprovedDate
            };
        }

        public void Update(Entities.ApplicationStateChangelog source, ApplicationStateChangelogDto target)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            source.FirstApprovedDate ??= target.FirstApprovedDate;
            source.LastApprovedDate = target.LastApprovedDate;
        }
    }
}