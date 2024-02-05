using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;
using AuditEntryDto = WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos.AuditEntryDto;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers
{
    public interface IAuditMapper
    {
        ODataQueryContext Map(ODataQueryParameters queryParameters);

        AuditEntryDto Map(Core.Contracts.Dtos.AuditEntryDto auditEntryDto);
    }

    internal class AuditMapper : IAuditMapper
    {
        private readonly IInitiationMapper _initiationMapper;

        public AuditMapper(IInitiationMapper initiationMapper)
        {
            _initiationMapper = initiationMapper ?? throw new ArgumentNullException(nameof(initiationMapper));
        }

        public ODataQueryContext Map(ODataQueryParameters queryParameters)
        {
            if (queryParameters is null)
                throw new ArgumentNullException(nameof(queryParameters));

            return new ODataQueryContext
            {
                OrderBy = queryParameters.OrderBy,
                Filter = queryParameters.Filter,
                Skip = queryParameters.Skip?.ToString(),
                Top = queryParameters.Top?.ToString()
            };
        }

        public AuditEntryDto Map(Core.Contracts.Dtos.AuditEntryDto auditEntryDto)
        {
            if (auditEntryDto == null)
                throw new ArgumentNullException(nameof(auditEntryDto));

            return new AuditEntryDto
            {
                UserId = auditEntryDto.UserId,
                EntryKey = auditEntryDto.EntryKey,
                EntryType = auditEntryDto.EntryType,
                EventType = auditEntryDto.EventType,
                CreatedAt = auditEntryDto.CreatedAt,
                Data = auditEntryDto.Data,
                Initiation = _initiationMapper.Map(auditEntryDto.Initiation)
            };
        }
    }
}