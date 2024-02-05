using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Domain;

namespace WX.B2C.User.Verification.Core.Services.Mappers
{
    public interface IInitiationMapper
    {
        Initiation Map(InitiationDto initiationDto);
    }

    internal class InitiationMapper
        : IInitiationMapper
    {
        public Initiation Map(InitiationDto initiationDto)
        {
            if (initiationDto == null)
                throw new ArgumentNullException(nameof(initiationDto));

            return new Initiation(initiator: initiationDto.Initiator, reason: initiationDto.Reason);
        }
    }
}
