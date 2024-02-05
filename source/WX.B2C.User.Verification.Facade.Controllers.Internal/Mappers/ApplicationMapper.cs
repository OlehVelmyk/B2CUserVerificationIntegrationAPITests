using System;
using WX.B2C.User.Verification.Facade.Controllers.Internal.Dtos;

namespace WX.B2C.User.Verification.Facade.Controllers.Internal.Mappers
{
    public interface IApplicationMapper
    {
        ApplicationDto Map(Core.Contracts.Dtos.ApplicationDto application, Core.Contracts.Dtos.ApplicationStateChangelogDto applicationStateChangelogDto);
    }

    internal class ApplicationMapper : IApplicationMapper
    {
        public ApplicationDto Map(Core.Contracts.Dtos.ApplicationDto application, Core.Contracts.Dtos.ApplicationStateChangelogDto applicationStateChangelogDto)
        {
            if (application == null)
                throw new ArgumentNullException(nameof(application));

            return new ApplicationDto
            {
                Id = application.Id,
                UserId = application.UserId,
                ProductType = application.ProductType,
                State = application.State,
                PreviousState = application.PreviousState,
                DecisionReasons = application.DecisionReasons,
                CreatedAt = application.CreatedAt,
                FirstApprovedDate = applicationStateChangelogDto?.FirstApprovedDate,
                LastApprovedDate = applicationStateChangelogDto?.LastApprovedDate
            };
        }
    }
}