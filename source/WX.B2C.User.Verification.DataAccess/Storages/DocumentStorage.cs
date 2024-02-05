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
using WX.B2C.User.Verification.DataAccess.Entities;
using WX.B2C.User.Verification.DataAccess.Mappers;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.DataAccess.Storages
{
    internal class DocumentStorage : IDocumentStorage
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IDocumentMapper _mapper;

        public DocumentStorage(IDbContextFactory dbContextFactory, IDocumentMapper mapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<DocumentDto> GetAsync(Guid documentId)
        {
            var predicate = FilterById(documentId);

            await using var dbContext = _dbContextFactory.Create();
            var query = QueryWithFiles(dbContext, predicate);
            var document = await query.FirstOrDefaultAsync();

            return document == null
                ? throw EntityNotFoundException.ByKey<Document>(documentId)
                : _mapper.Map(document);
        }
        
        public async Task<DocumentDto> FindAsync(Guid documentId, Guid userId)
        {
            var predicate = FilterById(documentId);

            await using var dbContext = _dbContextFactory.Create();
            var query = QueryWithFiles(dbContext, predicate);
            var document = await query.FirstOrDefaultAsync();

            return document?.UserId != userId ? null : _mapper.Map(document);
        }

        public async Task<DocumentDto[]> FindAsync(Guid userId, DocumentCategory? category)
        {
            var predicate = FilterByUserIdAndCategory(userId, category);

            await using var dbContext = _dbContextFactory.Create();
            var query = QueryWithFiles(dbContext, predicate);
            var documents = await query.ToArrayAsync();

            return documents.Select(_mapper.Map).ToArray();
        }

        public async Task<DocumentDto> FindLatestAsync(Guid userId, DocumentCategory category)
        {
            var predicate = FilterByUserIdAndCategory(userId, category);

            await using var dbContext = _dbContextFactory.Create();
            var query = QueryWithFiles(dbContext, predicate);
            var document = await query.FirstOrDefaultAsync();

            return document is null ? null : _mapper.Map(document);
        }

        public async Task<DocumentDto[]> FindSubmittedDocumentsAsync(Guid userId)
        {
            var predicate = FilterSubmittedByUserId(userId);

            await using var dbContext = _dbContextFactory.Create();
            var query = QueryWithFiles(dbContext, predicate);
            var documents = await query.ToArrayAsync();

            return documents.Select(_mapper.Map).ToArray();
        }

        public async Task<DocumentDto[]> FindSubmittedDocumentsAsync(Guid userId, IEnumerable<DocumentCategory> categories)
        {
            var predicate = FilterSubmittedByUserId(userId).And(FilterByCategories(categories));

            await using var dbContext = _dbContextFactory.Create();
            var query = QueryWithFiles(dbContext, predicate);
            var documents = await query.ToArrayAsync();

            return documents.Select(_mapper.Map).ToArray();
        }

        private static IQueryable<Document> QueryWithFiles(DbContext dbContext, Expression<Func<Document, bool>> predicate = null)
        {
            return dbContext
                   .QueryAsNoTracking(predicate)
                   .OrderByDescending(document => document.CreatedAt)
                   .Include(document => document.Files)
                   .ThenInclude(document => document.File);
        }

        private static Expression<Func<Document, bool>> FilterById(Guid documentId)
            => document => document.Id == documentId;

        private static Expression<Func<Document, bool>> FilterByCategories(IEnumerable<DocumentCategory> categories)
        {
            if (categories == null)
                throw new ArgumentNullException(nameof(categories));

            return document => categories.Contains(document.Category);
        }

        private static Expression<Func<Document, bool>> FilterSubmittedByUserId(Guid userId)
            => document => document.UserId == userId && document.Status == DocumentStatus.Submitted;

        private static Expression<Func<Document, bool>> FilterByUserIdAndCategory(Guid userId, DocumentCategory? documentCategory) 
            => document => document.UserId == userId && (documentCategory == null || document.Category == documentCategory);
    }
}