using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Core.Contracts.Exceptions;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities.Policy;
using WX.B2C.User.Verification.DataAccess.Mappers;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.DataAccess.Storages
{
    internal class ValidationPolicyStorage : IValidationPolicyStorage
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IValidationRuleMapper _mapper;

        public ValidationPolicyStorage(IDbContextFactory dbContextFactory, IValidationRuleMapper mapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Dictionary<ActionType, ValidationRuleDto>> GetAsync(ValidationPolicySelectionContext selectionContext)
        {
            var predicate = FilterBySelectionContext(selectionContext);

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext
                        .QueryAsNoTracking(predicate)
                        .OrderByDescending(verificationPolicy => verificationPolicy.RegionType)
                        .Include(validationPolicy => validationPolicy.Rules)
                        .ThenInclude(rule => rule.Rule);
            var policies = await query.ToArrayAsync();

            return policies.Length == 0  
                ? throw EntityNotFoundException.ByQuery<ValidationPolicy>(selectionContext)
                : _mapper.Map(policies);
        }

        private static Expression<Func<ValidationPolicy, bool>> FilterBySelectionContext(ValidationPolicySelectionContext selectionContext)
        {
            if (selectionContext == null)
                throw new ArgumentNullException(nameof(selectionContext));

            return policy => policy.RegionType == RegionType.Country && policy.Region == selectionContext.Country ||
                             policy.RegionType == RegionType.Region && policy.Region == selectionContext.Region ||
                             policy.RegionType == RegionType.Global;
        }
    }
}

