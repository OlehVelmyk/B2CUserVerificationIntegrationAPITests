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
using CheckDto = WX.B2C.User.Verification.Core.Contracts.Dtos.CheckDto;

namespace WX.B2C.User.Verification.Facade.Controllers.Webhook.Handlers
{
    public class CompletePassFortCheck : IRequest
    {
        public string CheckId { get; set; }

        public string CheckType { get; set; }

        public string CheckResult { get; set; }
    }

    internal class CompletePassFortCheckHandler : ForgettableHandler<CompletePassFortCheck>
    {
        private readonly ICheckStorage _checkStorage;
        private readonly ICheckService _checkService;

        public CompletePassFortCheckHandler(
            ICheckStorage checkStorage,
            ICheckService checkService,
            ILogger logger) : base(logger)
        {
            _checkStorage = checkStorage ?? throw new ArgumentNullException(nameof(checkStorage));
            _checkService = checkService ?? throw new ArgumentNullException(nameof(checkService));
        }

        protected override async Task Handle(CompletePassFortCheck request)
        {
            const CheckProviderType checkProvider = CheckProviderType.PassFort;
            var checkId = request.CheckId;
            var foundChecks = await _checkStorage.FindByExternalIdAsync(checkId, checkProvider);
            if (!foundChecks.Any())
            {
                _logger.ForContext(nameof(request), request, true)
                       .Warning(LogMessages.CheckNotFound, checkId, checkProvider);
                return;
            }

            await foundChecks.Select(ProcessAsync).WhenAll();
        }

        private Task ProcessAsync(CheckDto check)
        {
            if (check == null)
                throw new ArgumentNullException(nameof(check));

            if (check.State != CheckState.Running)
                _logger.Warning("Receive suspicious webhook from PassFort. Finish check {Id} with state {State}", check.Id, check.State);

            return _checkService.FinishExecutionAsync(check.Id, new CheckExecutionResultDto());
        }
    }
}
