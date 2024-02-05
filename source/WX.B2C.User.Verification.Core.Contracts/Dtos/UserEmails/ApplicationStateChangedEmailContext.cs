using System;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos.UserEmails
{
    public class ApplicationStateChangedEmailContext
    {
        public Guid UserId { get; set; }

        public ApplicationState NewState { get; set; }
    }
}