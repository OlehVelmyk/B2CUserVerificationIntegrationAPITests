using System.Collections.Generic;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos.Ticket
{
    public class TicketDto
    {
        public string Reason { get; set; }
        
        public IReadOnlyDictionary<string, string> Parameters { get; set; }
    }
}