using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WX.B2C.User.Verification.Core.Contracts.Exceptions;
using WX.B2C.User.Verification.DataAccess.Mappers;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.DataAccess.Repositories
{
    using Predicate = Expression<Func<Entities.Application, bool>>;
    
    internal class ApplicationRepository : IApplicationRepository
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IApplicationMapper _mapper;

        public ApplicationRepository(IDbContextFactory dbContextFactory, IApplicationMapper mapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Application> FindAsync(Guid userId, ProductType productType)
        {
            await using var dbContext = _dbContextFactory.Create();
            var entity = await FindAsync(dbContext, userId, productType);

            return entity == null
                ? null
                : _mapper.Map(entity);
        }

        public async Task<Application> GetAsync(Guid id)
        {
            await using var dbContext = _dbContextFactory.Create();
            var entity = await FindAsync(dbContext, id);

            return entity == null
                ? throw EntityNotFoundException.ByKey<Entities.Application>(id)
                : _mapper.Map(entity);
        }

        public async Task SaveAsync(Application application)
        {
            await using var dbContext = _dbContextFactory.Create();
            var entity = await FindAsync(dbContext, application.Id);

            if (entity == null)
                dbContext.Add(_mapper.Map(application));
            else
            {
                _mapper.Update(application, entity);
                dbContext.Update(entity);
            }

            await dbContext.SaveChangesAsync();
        }

        private static Task<Entities.Application> FindAsync(DbContext dbContext, Guid id)
        {
            var predicate = FilterById(id);
            
            var query = dbContext
                   .Set<Entities.Application>()
                   .Include(x => x.RequiredTasks)
                   .ThenInclude(x => x.Task)
                   .Where(predicate);

            return query.SingleOrDefaultAsync();
        }

        private static Task<Entities.Application> FindAsync(DbContext dbContext, Guid userId, ProductType productType)
        {
            var predicate = FilterByUser(userId)
                            .And(FilterByProductType(productType))
                            .And(FilterNotRejected());
            
            var query = dbContext
                        .Set<Entities.Application>()
                        .Include(x => x.RequiredTasks)
                        .ThenInclude(x => x.Task)
                        .Where(predicate);

            return query.SingleOrDefaultAsync();
        }
        
        private static Predicate FilterByUser(Guid userId) =>
            application => application.UserId == userId;
        
        private static Predicate FilterByProductType(ProductType productType) =>
            application => application.ProductType == productType;
        
        private static Predicate FilterById(Guid id) =>
            application => application.Id == id;

        private static Predicate FilterNotRejected() =>
            application => application.State != ApplicationState.Rejected && 
                           application.State != ApplicationState.Cancelled;
    }
}