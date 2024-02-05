using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public class Ticket
    {
        private Ticket(Guid userId, string reason, IReadOnlyDictionary<string, string> parameters)
        {
            UserId = userId;
            Reason = reason;
            Parameters = parameters;
        }

        public static Ticket Create(Guid userId, 
                                    string reason, 
                                    IReadOnlyDictionary<string, string> parameters)
        {
            return new Ticket(userId, reason, parameters);
        }

        public Guid UserId { get; }

        public string Reason { get; }

        public IReadOnlyDictionary<string,string> Parameters { get; }
    }


    public interface ITicketSender
    {
        Task SendAsync(Ticket ticket);
    }
}