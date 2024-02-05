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
    internal class DocumentRepository : IDocumentRepository
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IDocumentMapper _mapper;

        public DocumentRepository(IDbContextFactory dbContextFactory, IDocumentMapper mapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<DocumentDto> GetAsync(Guid documentId)
        {
            await using var dbContext = _dbContextFactory.Create();
            var entity = await FindAsync(dbContext, documentId);

            return entity == null
                ? throw EntityNotFoundException.ByKey<Document>(documentId)
                : _mapper.Map(entity);
        }

        public async Task SaveAsync(DocumentDto documentDto)
        {
            await using var dbContext = _dbContextFactory.Create();
            var entity = await FindAsync(dbContext, documentDto.Id);

            if (entity == null)
            {
                entity = _mapper.Map(documentDto);
                dbContext.Add(entity);
            }
            else
            {
                _mapper.Update(documentDto, entity);
                dbContext.Update(entity);
            }

            await dbContext.SaveChangesAsync();
        }

        private static Task<Document> FindAsync(DbContext dbContext, Guid id)
        {
            var query = dbContext
                        .Set<Document>()
                        .Include(x => x.Files)
                        .ThenInclude(x => x.File)
                        .Where(document => document.Id == id);

            return query.SingleOrDefaultAsync();
        }
    }
}