using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities;
using WX.B2C.User.Verification.DataAccess.Mappers;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.DataAccess.Storages
{
    internal class ReminderStorage : IReminderStorage
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IReminderMapper _mapper;

        public ReminderStorage(IDbContextFactory dbContextFactory, IReminderMapper mapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<UserReminderDto[]> FindAsync(Guid userId)
        {
            var predicate = FilterByUserId(userId);

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext
                        .QueryAsNoTracking(predicate)
                        .OrderByDescending(reminder => reminder.SentAt);
            var reminders = await query.ToArrayAsync();

            return reminders.Select(_mapper.Map).ToArray();
        }

        public async Task<int> CountAsync(Guid userId, Guid targetId)
        {
            var predicate = FilterByUserId(userId).And(FilterByTargetId(targetId));

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext
                        .QueryAsNoTracking(predicate);

            return await query.CountAsync();
        }

        private static Expression<Func<Reminder, bool>> FilterByUserId(Guid userId) =>
            reminder => reminder.UserId == userId;
        
        private static Expression<Func<Reminder, bool>> FilterByTargetId(Guid targetId) =>
            reminder => reminder.TargetId == targetId;
    }
}
