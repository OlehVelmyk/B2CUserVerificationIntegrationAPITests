using System;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers
{
    public interface IInitiationMapper
    {
        Core.Contracts.Dtos.InitiationDto Map(InitiationDto initiationDto);

        InitiationDto Map(Core.Contracts.Dtos.InitiationDto initiationDto);
    }

    internal class InitiationMapper : IInitiationMapper
    {
        public Core.Contracts.Dtos.InitiationDto Map(InitiationDto initiationDto)
        {
            if (initiationDto == null)
                throw new ArgumentNullException(nameof(initiationDto));

            return new Core.Contracts.Dtos.InitiationDto
            {
                Initiator = initiationDto.Initiator,
                Reason = initiationDto.Reason
            };
        }

        public InitiationDto Map(Core.Contracts.Dtos.InitiationDto initiationDto)
        {
            if (initiationDto == null)
                throw new ArgumentNullException(nameof(initiationDto));

            return new InitiationDto
            {
                Initiator = initiationDto.Initiator,
                Reason = initiationDto.Reason
            };
        }
    }
}