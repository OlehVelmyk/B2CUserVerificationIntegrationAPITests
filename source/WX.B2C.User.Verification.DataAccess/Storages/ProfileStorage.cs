using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Exceptions;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities;
using WX.B2C.User.Verification.DataAccess.Mappers;

namespace WX.B2C.User.Verification.DataAccess.Storages
{
    internal class ProfileStorage : IProfileStorage
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IVerificationDetailsMapper _verificationDetailsMapper;
        private readonly IPersonalDetailsMapper _personalDetailsMapper;

        public ProfileStorage(IDbContextFactory dbContextFactory,
                              IVerificationDetailsMapper verificationDetailsMapper,
                              IPersonalDetailsMapper personalDetailsMapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _verificationDetailsMapper = verificationDetailsMapper ?? throw new ArgumentNullException(nameof(verificationDetailsMapper));
            _personalDetailsMapper = personalDetailsMapper ?? throw new ArgumentNullException(nameof(personalDetailsMapper));
        }

        public async Task<VerificationDetailsDto> FindVerificationDetailsAsync(Guid userId)
        {
            var predicate = FilterVerificationDetails(userId);

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate);
            var verificationDetails = await query.FirstOrDefaultAsync();
            
            return verificationDetails == null 
                ? null 
                : _verificationDetailsMapper.Map(verificationDetails);
        }

        public async Task<VerificationDetailsDto> GetVerificationDetailsAsync(Guid userId)
        {
            var details = await FindVerificationDetailsAsync(userId);
            return details ?? throw EntityNotFoundException.ByKey<VerificationDetails>(userId);
        }

        public async Task<string> GetResidenceCountryAsync(Guid userId)
        {
            var predicate = FilterResidenceAddress(userId);

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext
                        .QueryAsNoTracking(predicate)
                        .Select(address => new { address.Country, address.UserId });
            var residenceAddress = await query.FirstOrDefaultAsync();

            return residenceAddress?.Country ?? throw EntityNotFoundException.ByKey<ResidenceAddress>(userId);
        }

        public async Task<PersonalDetailsDto> FindPersonalDetailsAsync(Guid userId)
        {
            var predicate = FilterPersonalDetails(userId);

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext
                        .QueryAsNoTracking(predicate)
                        .Include(details => details.ResidenceAddress);
            var personalDetails = await query.FirstOrDefaultAsync();

            return personalDetails == null 
                ? null 
                : _personalDetailsMapper.Map(personalDetails);
        }
        
        public async Task<PersonalDetailsDto> GetPersonalDetailsAsync(Guid userId)
        {
            var personalDetails = await FindPersonalDetailsAsync(userId);
            return personalDetails ?? throw EntityNotFoundException.ByKey<PersonalDetails>(userId);
        }

        public async Task<PersonalDetailsDto> GetPersonalDetailsByExternalProfileIdAsync(string externalProfileId)
        {
            var predicate = FilterByExternalId();

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate);
            var externalProfile = await query.FirstOrDefaultAsync();

            return await GetPersonalDetailsAsync(externalProfile.UserId);

            Expression<Func<ExternalProfile, bool>> FilterByExternalId() =>
                ep => ep.ExternalId == externalProfileId;
        }

        public async Task<AddressDto> GetResidenceAddressAsync(Guid userId)
        {
            var residenceAddress = await FindResidenceAddressAsync(userId);
            return residenceAddress ?? throw EntityNotFoundException.ByKey<ResidenceAddress>(userId);
        }

        public async Task<AddressDto> FindResidenceAddressAsync(Guid userId)
        {
            var predicate = FilterResidenceAddress(userId);

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate);
            var residenceAddress = await query.FirstOrDefaultAsync();

            return _personalDetailsMapper.SafeMap(residenceAddress);
        }

        public async Task<VerificationDetailsDto[]> GetVerificationDetailsAsync(Guid[] userIds)
        {
            var predicate = FilterVerificationDetails(userIds);

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate);
            var verificationDetails = await query.ToArrayAsync();

            return verificationDetails.Select(_verificationDetailsMapper.Map).ToArray();
        }

        private static Expression<Func<PersonalDetails, bool>> FilterPersonalDetails(Guid userId)
            => profile => profile.UserId == userId;

        private static Expression<Func<ResidenceAddress, bool>> FilterResidenceAddress(Guid userId)
            => profile => profile.UserId == userId;

        private static Expression<Func<VerificationDetails, bool>> FilterVerificationDetails(Guid userId)
            => profile => profile.UserId == userId;

        private static Expression<Func<VerificationDetails, bool>> FilterVerificationDetails(Guid[] userIds)
            => profile => userIds.Contains(profile.UserId);
    }
}