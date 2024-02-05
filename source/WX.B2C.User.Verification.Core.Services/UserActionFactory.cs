using System;
using System.Linq;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Core.Services
{
    public class UserActionFactory : IUserActionFactory
    {
        public UserActionDto Create(string xPath, bool isRequired, ActionOption actionOption)
        {
            if (actionOption == null)
                throw new ArgumentNullException(nameof(actionOption));
            if (xPath == null)
                throw new ArgumentNullException(nameof(xPath));
            if (actionOption.XPath != xPath)
                throw new ArgumentException("Action option must have same xPath as collection step", nameof(actionOption));

            return new UserActionDto
            {
                ActionType = actionOption.ActionType,
                Reason = "Unrestricted",
                Priority = actionOption.Priority,
                IsOptional = !isRequired,
                ActionData = actionOption.Metadata?.ToDictionary(pair => pair.Key, pair => pair.Value)
            };
        }
    }
}