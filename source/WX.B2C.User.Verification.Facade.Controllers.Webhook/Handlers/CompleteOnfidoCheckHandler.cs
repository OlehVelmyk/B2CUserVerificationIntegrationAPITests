using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Facade.Controllers.Webhook.Handlers
{
    public class CompleteOnfidoCheck : IRequest
    {
        private CompleteOnfidoCheck()
        {
        }

        public string ApplicantId { get; private set; }

        public string CheckId { get; private set; }

        public static CompleteOnfidoCheck Create(string applicantId, string checkId)
        {
            return new CompleteOnfidoCheck
            {
                ApplicantId = applicantId,
                CheckId = checkId
            };
        }
    }

    internal class CompleteOnfidoCheckHandler : ForgettableHandler<CompleteOnfidoCheck>
    {
        private readonly ICheckStorage _checkStorage;
        private readonly ICheckService _checkService;

        public CompleteOnfidoCheckHandler(
            ICheckStorage checkStorage,
            ICheckService checkService,
            ILogger logger) : base(logger)
        {
            _checkStorage = checkStorage ?? throw new ArgumentNullException(nameof(checkStorage));
            _checkService = checkService ?? throw new ArgumentNullException(nameof(checkService));
        }

        protected override async Task Handle(CompleteOnfidoCheck request)
        {
            // Resolve check by externalId
            const CheckProviderType checkProvider = CheckProviderType.Onfido;
            var foundChecks = await _checkStorage.FindByExternalIdAsync(request.CheckId, checkProvider);
            if (!foundChecks.Any())
            {
                _logger.ForContext(nameof(request), request, true)
                       .Warning(LogMessages.CheckNotFound, request.CheckId, checkProvider);
                return;

            }

            await foundChecks.Select(ProcessAsync).WhenAll();
        }

        private Task ProcessAsync(CheckDto check)
        {
            if (check == null)
                throw new ArgumentNullException(nameof(check));

            if (check.State != CheckState.Running)
                _logger.Warning("Receive suspicious webhook from Onfido. Finish check {Id} with state {State}", check.Id, check.State);

            return _checkService.FinishExecutionAsync(check.Id, new CheckExecutionResultDto());
        }
    }
}
