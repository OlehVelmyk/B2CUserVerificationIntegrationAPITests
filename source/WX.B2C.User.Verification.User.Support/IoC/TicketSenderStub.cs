using System;
using System.Threading.Tasks;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts;

namespace WX.B2C.User.Verification.User.Support.IoC
{
    internal class TicketSenderStub:ITicketSender
    {
        private readonly ILogger _logger;

        public TicketSenderStub(ILogger logger)
        {
            _logger = logger?.ForContext<TicketSenderStub>() ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public Task SendAsync(Ticket ticket)
        {
            _logger.Information("Send ticket {Reason} and from user {UserId}", ticket.Reason, ticket.UserId);
            return Task.CompletedTask;
        }
    }
}