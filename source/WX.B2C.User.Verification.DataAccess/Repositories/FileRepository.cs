using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Exceptions;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.DataAccess.Entities;
using WX.B2C.User.Verification.DataAccess.Mappers;

namespace WX.B2C.User.Verification.DataAccess.Repositories
{
    internal class FileRepository : IFileRepository
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IFileMapper _mapper;

        public FileRepository(IDbContextFactory dbContextFactory, IFileMapper mapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<FileDto> GetAsync(Guid fileId)
        {
            await using var dbContext = _dbContextFactory.Create();
            var entity = await FindAsync(dbContext, fileId);

            return entity == null
                ? throw EntityNotFoundException.ByKey<File>(fileId)
                : _mapper.Map(entity);
        }

        public async Task SaveAsync(FileDto fileDto)
        {
            await using var dbContext = _dbContextFactory.Create();
            var entity = await FindAsync(dbContext, fileDto.Id);

            if (entity == null)
            {
                entity = _mapper.Map(fileDto);
                dbContext.Add(entity);
            }
            else
            {
                _mapper.Update(fileDto, entity);
                dbContext.Update(entity);
            }

            await dbContext.SaveChangesAsync();
        }

        private static Task<File> FindAsync(DbContext dbContext, Guid id)
        {
            var query = dbContext
                        .Set<File>()
                        .Where(file => file.Id == id);

            return query.SingleOrDefaultAsync();
        }
    }
}
