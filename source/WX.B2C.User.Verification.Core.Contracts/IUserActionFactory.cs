using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface IUserActionFactory
    {
        /// <summary>
        /// Creates <see cref="UserActionDto"/> if it is verification domain action.
        /// </summary>
        UserActionDto Create(string xPath, bool isRequired, ActionOption actionOption);
    }
}