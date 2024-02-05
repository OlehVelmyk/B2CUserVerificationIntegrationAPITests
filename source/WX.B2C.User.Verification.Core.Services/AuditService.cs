using System;
using System.Threading.Tasks;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Core.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditEntryRepository _auditEntryRepository;
        private readonly ILogger _logger;

        public AuditService(IAuditEntryRepository auditEntryRepository, ILogger logger)
        {
            _auditEntryRepository = auditEntryRepository ?? throw new ArgumentNullException(nameof(auditEntryRepository));
            _logger = logger.ForContext<AuditService>() ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task SaveAsync(AuditEntryDto auditDto)
        {
            if (auditDto == null)
                throw new ArgumentNullException(nameof(auditDto));

            _auditEntryRepository.SaveAsync(auditDto).RunAndForget();
            return Task.CompletedTask;
        }
    }
}