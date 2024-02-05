using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Triggers.Configs;

namespace WX.B2C.User.Verification.Core.Contracts.Triggers
{
    public interface ITriggerCommandRunner
    {
        Task RunAsync(Guid userId,
                      Guid applicationId,
                      Guid triggerId,
                      CommandConfig[] commands,
                      InitiationDto initiation);
    }
}