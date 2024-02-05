using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Query;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Admin.OData;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Validators
{
    public class AuditODataValidator : BaseODataValidator<AuditEntryDto>
    {
        public AuditODataValidator(IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
        }

        protected override ODataValidationSettings GetValidationSettings()
        {
            return new ODataValidationSettings
            {
                AllowedLogicalOperators = AllowedLogicalOperators.Equal
                                          | AllowedLogicalOperators.NotEqual
                                          | AllowedLogicalOperators.Not
                                          | AllowedLogicalOperators.And
                                          | AllowedLogicalOperators.Or,
                AllowedQueryOptions = AllowedQueryOptions.Filter
                                      | AllowedQueryOptions.OrderBy
                                      | AllowedQueryOptions.Skip
                                      | AllowedQueryOptions.Top,
                AllowedFilterProperties = { nameof(AuditEntryDto.EntryType), nameof(AuditEntryDto.EventType) },
                AllowedOrderByProperties = { nameof(AuditEntryDto.CreatedAt) }
            };
        }
    }
}
