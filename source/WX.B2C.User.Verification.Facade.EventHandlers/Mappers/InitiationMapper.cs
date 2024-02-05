using System;
using WX.B2C.User.Verification.Events.Dtos;
using Internal = WX.B2C.User.Verification.Events.Internal.Dtos;

namespace WX.B2C.User.Verification.Facade.EventHandlers.Mappers
{
    public interface IInitiationMapper
    {
        Core.Contracts.Dtos.InitiationDto Map(Internal.InitiationDto initiationDto);

        Core.Contracts.Dtos.InitiationDto Map(InitiationDto initiationDto);
    }

    internal class InitiationMapper : IInitiationMapper
    {
        public Core.Contracts.Dtos.InitiationDto Map(Internal.InitiationDto initiationDto)
        {
            if (initiationDto == null)
                throw new ArgumentNullException(nameof(initiationDto));

            return Core.Contracts.Dtos.InitiationDto.Create(initiationDto.Initiator, initiationDto.Reason);
        }

        public Core.Contracts.Dtos.InitiationDto Map(InitiationDto initiationDto)
        {
            if (initiationDto == null)
                throw new ArgumentNullException(nameof(initiationDto));

            return Core.Contracts.Dtos.InitiationDto.Create(initiationDto.Initiator, initiationDto.Reason);
        }
    }
}