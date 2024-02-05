using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Exceptions;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.DataAccess.EF.Extensions;
using WX.B2C.User.Verification.DataAccess.Entities;
using WX.B2C.User.Verification.DataAccess.Mappers;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.DataAccess.Storages
{
    internal class FileStorage : IFileStorage
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IFileMapper _mapper;

        public FileStorage(IDbContextFactory dbContextFactory, IFileMapper mapper)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<FileDto> GetAsync(Guid fileId)
        {
            var predicate = FilterByFileId(fileId);

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate);
            var file = await query.FirstOrDefaultAsync();

            return file == null
                ? throw EntityNotFoundException.ByKey<File>(fileId)
                : _mapper.Map(file);
        }

        public async Task<FileDto> FindAsync(Guid userId, Guid fileId)
        {
            var predicate = FilterByFileId(fileId);

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate);
            var file = await query.FirstOrDefaultAsync();

            return file?.UserId != userId
                ? null
                : _mapper.Map(file);
        }

        public async Task<FileDto> FindAsync(Guid userId, uint crc32Checksum)
        {
            var predicate = FilterByUserId(userId).And(FilterByChecksum(crc32Checksum));

            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate);
            var file = await query.FirstOrDefaultAsync();

            return file == null
                ? null
                : _mapper.Map(file);
        }

        public async Task<string> GetDocumentTypeAsync(Guid fileId)
        {
            Expression<Func<DocumentFile, bool>> predicate = document => document.FileId == fileId;
            await using var dbContext = _dbContextFactory.Create();
            var query = dbContext.QueryAsNoTracking(predicate)
                                 .Include(documentFile => documentFile.Document)
                                 .Select(documentFile => documentFile.Document.Type)
                                 .FirstOrDefaultAsync();
            
            var documentType = await query;
            return documentType ?? throw EntityNotFoundException.ByQuery<DocumentFile>(new { fileId });
        }

        private static Expression<Func<File, bool>> FilterByFileId(Guid fileId)
            => file => file.Id == fileId;

        private static Expression<Func<File, bool>> FilterByUserId(Guid userId)
            => file => file.UserId == userId;

        private static Expression<Func<File, bool>> FilterByChecksum(uint crc32Checksum)
            => file => file.Crc32Checksum == crc32Checksum;
    }
}
