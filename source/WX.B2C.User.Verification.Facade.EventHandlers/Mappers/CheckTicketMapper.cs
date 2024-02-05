using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Ticket;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Events.Internal.EventArgs;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Facade.EventHandlers.Mappers
{
    internal interface ICompletedCheckMapper
    {
        CheckTicketContext Map(CheckCompletedEventArgs eventArgs);
    }

    internal class CheckTicketMapper : ICompletedCheckMapper
    {
        public CheckTicketContext Map(CheckCompletedEventArgs eventArgs)
        {
            if (eventArgs == null)
                throw new ArgumentNullException(nameof(eventArgs));

            return new CheckTicketContext
            {
                UserId = eventArgs.UserId,
                CheckId = eventArgs.CheckId,
                Result = eventArgs.Result.To<CheckResult>(),
                Type = eventArgs.Type.To<CheckType>(),
                State = CheckState.Complete,
                Provider = eventArgs.Provider.To<CheckProviderType>(),
                Decision = eventArgs.Decision
            };
        }
    }
}