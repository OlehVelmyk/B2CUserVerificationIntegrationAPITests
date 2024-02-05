using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities;
using WX.B2C.User.Verification.Extensions;
using EFCore = Microsoft.EntityFrameworkCore.EF;

namespace WX.B2C.User.Verification.DataAccess.Storages
{
    internal class DuplicateSearchService : IDuplicateSearchService
    {
        private readonly IDbContextFactory _dbContextFactory;
        private const int MaxNumberOfMatchesToTake = 10;

        public DuplicateSearchService(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        }

        public async Task<DuplicateSearchResult> FindAsync(DuplicateSearchContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // Find duplicates by various criteria.
            var searchResults = await new[]
            {
                FindByNameAndBirthDateAsync(context.UserId, context.FullName, context.BirthDate),
                FindByIdDocumentNumberAsync(context.UserId, context.IdDocumentNumber)
            }.WhenAll();

            // Squash duplicates for the same user into single result.
            var squashedMatches = searchResults
                .SelectMany(result => result.Matches)
                .Aggregate(
                    new List<DuplicateMatch>(),
                    AddOrSquashByUserId,
                    results => results.ToArray());

            // TODO: https://wirexapp.atlassian.net/browse/WRXB-10701
            // Populate details about duplicated users.
            squashedMatches = await Populate(squashedMatches);

            var total = searchResults.Select(result => result.Total).Sum();
            return DuplicateSearchResult.Create(squashedMatches, total);
        }

        private async Task<DuplicateSearchResult> FindByNameAndBirthDateAsync(Guid userId, FullNameDto fullName, DateTime? dateOfBirth)
        {
            if (fullName == null || !dateOfBirth.HasValue)
                return DuplicateSearchResult.Empty;

            var expression = FindByNameAndBirthDate(userId, fullName, dateOfBirth.Value);

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext
                        .QueryAsNoTracking(expression)
                        .Select(x => new DuplicateMatch
                        {
                            UserId = x.UserId,
                            DuplicateType = DuplicateType.ByNameAndBirthDate
                        });

            var total = await query.CountAsync();
            var matches = await query.Take(MaxNumberOfMatchesToTake).ToArrayAsync();

            return DuplicateSearchResult.Create(matches, total);
        }

        private async Task<DuplicateSearchResult> FindByIdDocumentNumberAsync(Guid userId, IdDocumentNumberDto idDocumentNumber)
        {
            if (idDocumentNumber == null)
                return DuplicateSearchResult.Empty;

            var expression = FindByIdDocumentNumber(userId, idDocumentNumber);

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext
                        .QueryAsNoTracking(expression)
                        .Select(x => new DuplicateMatch
                        {
                            UserId = x.UserId,
                            DuplicateType = DuplicateType.ByIdDocumentNumber
                        });

            var total = await query.CountAsync();
            var matches = await query.Take(MaxNumberOfMatchesToTake).ToArrayAsync();

            return DuplicateSearchResult.Create(matches, total);
        }

        private static List<DuplicateMatch> AddOrSquashByUserId(List<DuplicateMatch> matches, DuplicateMatch otherMatch)
        {
            var match = matches.SingleOrDefault(x => x.UserId == otherMatch.UserId);

            if (match == null) matches.Add(otherMatch);
            else match.DuplicateType |= otherMatch.DuplicateType;

            return matches;
        }

        private async Task<DuplicateMatch[]> Populate(DuplicateMatch[] matches)
        {
            if (!matches.Any()) return Array.Empty<DuplicateMatch>();

            var distinctUserIds = matches.Select(x => x.UserId).Distinct().ToArray();

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext
                        .QueryAsNoTracking<PersonalDetails>()
                        .Where(x => distinctUserIds.Contains(x.UserId))
                        .Select(x => new { x.UserId, x.Email });
            var details = await query.ToArrayAsync();

            return matches.Select(match =>
            {
                var userDetails = details.FirstOrDefault(p => p.UserId == match.UserId);
                return match.WithEmail(userDetails?.Email);
            }).ToArray();
        }

        private static Expression<Func<PersonalDetails, bool>> FindByNameAndBirthDate(Guid userId, FullNameDto fullName, DateTime dateOfBirth)
        {
            return personalDetails => personalDetails.UserId != userId &&
                                      personalDetails.FirstName.ToUpper() == fullName.FirstName.ToUpper() &&
                                      personalDetails.LastName.ToUpper() == fullName.LastName.ToUpper() &&
                                      EFCore.Functions.DateDiffDay(personalDetails.DateOfBirth, dateOfBirth) == 0;
        }

        private static Expression<Func<VerificationDetails, bool>> FindByIdDocumentNumber(Guid userId, IdDocumentNumberDto idDocumentNumber)
        {
            return personalDetails => personalDetails.UserId != userId &&
                                      personalDetails.IdDocumentNumber.ToUpper() == idDocumentNumber.Number.ToUpper() &&
                                      personalDetails.IdDocumentNumberType == idDocumentNumber.Type;
        }
    }
}
