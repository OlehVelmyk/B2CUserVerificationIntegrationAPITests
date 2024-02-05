using System;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface IProfileProviderFactory
    {
        IProfileDataProvider Create(Guid userId);
    }
}