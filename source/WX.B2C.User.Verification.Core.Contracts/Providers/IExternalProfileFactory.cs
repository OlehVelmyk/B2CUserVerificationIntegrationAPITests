using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;

namespace WX.B2C.User.Verification.Core.Contracts.Module
{
    public interface IExternalProfileFactory
    {
        Task<ExternalProfileDto> CreateAsync(Guid userId);
    }
}