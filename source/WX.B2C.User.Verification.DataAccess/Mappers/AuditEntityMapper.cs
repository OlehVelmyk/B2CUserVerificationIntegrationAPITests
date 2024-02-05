using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.DataAccess.Entities.Audit;

namespace WX.B2C.User.Verification.DataAccess.Mappers
{
    internal interface IAuditEntityMapper
    {
        AuditEntry Map(AuditEntryDto auditEntryDto);

        AuditEntryDto MapToDto(AuditEntry auditEntry);
    }

    internal class AuditEntityMapper : IAuditEntityMapper
    {
        public AuditEntry Map(AuditEntryDto auditEntryDto)
        {
            if (auditEntryDto == null)
                throw new ArgumentNullException(nameof(auditEntryDto));

            return new AuditEntry
            {
                UserId = auditEntryDto.UserId,
                EntryKey = auditEntryDto.EntryKey,
                EntryType = auditEntryDto.EntryType,
                EventType = auditEntryDto.EventType,
                Data = auditEntryDto.Data,
                Initiator = auditEntryDto.Initiation.Initiator,
                Reason = auditEntryDto.Initiation.Reason,
                CreatedAt = auditEntryDto.CreatedAt
            };
        }

        public AuditEntryDto MapToDto(AuditEntry auditEntry)
        {
            if (auditEntry == null)
                throw new ArgumentNullException(nameof(auditEntry));

            var initiation = InitiationDto.Create(auditEntry.Initiator, auditEntry.Reason);

            return AuditEntryDto.Create(
                auditEntry.UserId, 
                auditEntry.EntryKey, 
                auditEntry.EntryType,
                auditEntry.EventType, 
                auditEntry.CreatedAt,
                auditEntry.Data,
                initiation);
        }
    }
}