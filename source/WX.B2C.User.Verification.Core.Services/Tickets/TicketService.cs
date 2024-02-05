using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Optional.Unsafe;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Ticket;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Checks;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.XPath;

namespace WX.B2C.User.Verification.Core.Services.Tickets
{
    internal class TicketService : ITicketService
    {
        private readonly IApplicationStorage _applicationStorage;
        private readonly IProfileStorage _profileStorage;
        private readonly ITicketInfoProvider _ticketInfoProvider;
        private readonly ITicketSender _ticketSender;
        private readonly ILogger _logger;
        private readonly ITicketFactory _ticketFactory;

        public TicketService(
            IApplicationStorage applicationStorage,
            IProfileStorage profileStorage,
            ITicketInfoProvider ticketInfoProvider,
            ITicketFactory ticketProvider,
            ITicketSender ticketSender,
            ILogger logger)
        {
            _applicationStorage = applicationStorage ?? throw new ArgumentNullException(nameof(applicationStorage));
            _profileStorage = profileStorage ?? throw new ArgumentNullException(nameof(profileStorage));
            _ticketInfoProvider = ticketInfoProvider ?? throw new ArgumentNullException(nameof(ticketInfoProvider));
            _ticketFactory = ticketProvider ?? throw new ArgumentNullException(nameof(ticketProvider));
            _ticketSender = ticketSender ?? throw new ArgumentNullException(nameof(ticketSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendAsync(CheckTicketContext context)
        {
            if (!HasPrerequisitesToSend(context)) return;

            var country = await _profileStorage.GetResidenceCountryAsync(context.UserId);
            var ticket = await GetTicketAsync(context, country);
            if (ticket == null) return; // TODO: Why null?

            await _ticketSender.SendAsync(Ticket.Create(context.UserId, ticket.Reason, ticket.Parameters));
        }

        public Task SendAsync(ApplicationTicketContext ticketContext)
        {
            return Task.CompletedTask; // TODO: Implement
        }

        public async Task SendAsync(TicketContext ticketContext)
        {
            var ticketDto = await _ticketFactory.CreateAsync(ticketContext.Reason, ticketContext.UserId);
            var ticket = Ticket.Create(ticketContext.UserId, ticketDto.Reason, ticketDto.Parameters);
            await _ticketSender.SendAsync(ticket);
        }

        public async Task SendAsync(ReviewCollectionStepTicketContext ticketContext)
        {
            //TODO open question for analytic - is it always PoA not onboarding? What to do with Surveys and their types? Maybe better to have universal template?
            //TODO later predefined tags can be moved to ticket templates. But dynamic should be calculated.
            var ticketReason = _ticketInfoProvider.FindAsync(ticketContext.XPath);
            if (!ticketReason.HasValue)
            {
                _logger.Warning("Sending review step ticket skipped for {XPath}", ticketContext.XPath);
                return;
            }

            var reason = ticketReason.ValueOrFailure();
            var ticketDto = await _ticketFactory.CreateAsync(reason, ticketContext.UserId);
            var ticket = Ticket.Create(ticketContext.UserId, ticketDto.Reason, ticketDto.Parameters);
            await _ticketSender.SendAsync(ticket);
        }

        public async Task SendAsync(AccountAlertTicketContext ticketContext)
        {
            var parameters = new Dictionary<string, object>
            {
                { XPathes.RiskLevel, ticketContext.RiskLevel },
                { XPathes.Turnover, ticketContext.Turnover },
                { nameof(ticketContext.ApplicationState), ticketContext.ApplicationState },
                { nameof(ticketContext.LastApprovedDate), ticketContext.LastApprovedDate },
            };
            var ticketDto = await _ticketFactory.CreateAsync(TicketReasons.AccountRefreshAlert, ticketContext.UserId, parameters);
            var ticket = Ticket.Create(ticketContext.UserId, ticketDto.Reason, ticketDto.Parameters);
            await _ticketSender.SendAsync(ticket);
        }

        public async Task SendAsync(ExceededCheckResubmitAttemptsTicketContext ticketContext)
        {
            var ticketReason = ticketContext.Type switch
            {
                CheckType.IdentityDocument => TicketReasons.PoIAttemptsExceeded,
                _ => null
            };

            if (ticketReason == null)
                return;

            var ticketDto = await _ticketFactory.CreateAsync(ticketReason, ticketContext.UserId);
            var ticket = Ticket.Create(ticketContext.UserId, ticketDto.Reason, ticketDto.Parameters);
            await _ticketSender.SendAsync(ticket);
        }

        private static bool HasPrerequisitesToSend(CheckTicketContext check)
        {
            return check.Result != CheckResult.Passed;
        }

        private async Task<TicketDto> GetTicketAsync(CheckTicketContext context, string country)
        {
            if (context.State == CheckState.Error)
                return await HandleCheckErrorOccurredAsync(context.UserId, context.Type.ToString());

            var templateName = context.Type switch
            {
                //TODO open question how to setup in policy that we have different templates for different countries
                CheckType.IpMatch
                    when country == "US" => TicketReasons.UsaIpMatchFailed,
                CheckType.IdentityDocument
                    when country == "US" => TicketReasons.UsaOnfidoCheckFailed,
                CheckType.FacialSimilarity
                    when country == "US "=> TicketReasons.UsaOnfidoCheckFailed,
                CheckType.IpMatch                => TicketReasons.IpMatchFailed,
                CheckType.NameAndDoBDuplication  => TicketReasons.NameAndDoBDuplication,
                CheckType.IdDocNumberDuplication => TicketReasons.IdDocNumberDuplication,
                CheckType.RiskListsScreening
                    when context.Provider == CheckProviderType.PassFort => TicketReasons.PassfortRiskListsScreening,
                CheckType.RiskListsScreening
                    when context.Provider == CheckProviderType.LexisNexis => TicketReasons.LexisNexisRiskListsScreening,
                CheckType.FraudScreening
                    when context.Provider == CheckProviderType.LexisNexis &&
                         context.Decision == CheckDecisions.PotentialFraud => TicketReasons.InstantIdCheckFailed,
                    _ => null
            };

            if (templateName == null)
                return null; //TODO is it possible to not have ticket: it seems that yes. 

            return await _ticketFactory.CreateAsync(templateName, context.UserId);
        }

        private async Task<TicketDto> HandleCheckErrorOccurredAsync(Guid userId, string checkType)
        {
            // TODO: Handle situation when we have several ProductTypes
            var applicationState = await _applicationStorage.GetStateAsync(userId, ProductType.WirexBasic);
            var context = new Dictionary<string, object>
            {
                { "ApplicationState", applicationState },
                { "CheckType", checkType }
            };
            return await _ticketFactory.CreateAsync(TicketReasons.CheckErrorDetected,
                                                    userId,
                                                    context);
        }
    }
}