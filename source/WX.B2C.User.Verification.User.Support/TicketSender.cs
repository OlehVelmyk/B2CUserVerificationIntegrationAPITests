using System;
using System.Threading.Tasks;
using WX.B2C.User.Support.Commands;
using WX.B2C.User.Support.Commands.Parameters;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.User.Support
{
    internal class TicketSender : ITicketSender
    {
        private readonly IUserSupportClient _client;
        private readonly IOperationContextProvider _operationContextProvider;

        public TicketSender(IUserSupportClient client, IOperationContextProvider operationContextProvider)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _operationContextProvider = operationContextProvider ?? throw new ArgumentNullException(nameof(operationContextProvider));
        }

        public Task SendAsync(Ticket ticket)
        {
            var parameters = CreateParameters(ticket);
            return _client.CreateNotifications(parameters);
        }

        private GenericNotificationParameters CreateParameters(Ticket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException(nameof(ticket));

            return new GenericNotificationParameters(ticket.UserId, CorrelationId, ticket.Reason, ticket.Parameters);
        }

        private Guid CorrelationId => _operationContextProvider.GetContextOrDefault().CorrelationId;
    }
}