using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;

namespace WX.B2C.User.Verification.Core.Contracts.Storages
{
    [Flags]
    public enum DuplicateType
    {
        ByNameAndBirthDate = 1,
        ByIdDocumentNumber = 2
    }

    public class DuplicateMatch
    {
        public Guid UserId { get; set; }

        public string Email { get; set; }

        public DuplicateType DuplicateType { get; set; }

        public DuplicateMatch WithEmail(string email)
        {
            Email = email;
            return this;
        }
    }

    public class DuplicateSearchContext
    {
        private DuplicateSearchContext(
            Guid userId,
            FullNameDto fullName, 
            DateTime? birthDate, 
            IdDocumentNumberDto idDocumentNumber)
        {
            UserId = userId;
            FullName = fullName;
            BirthDate = birthDate;
            IdDocumentNumber = idDocumentNumber;
        }

        public Guid UserId { get; }

        public FullNameDto FullName { get; }

        public DateTime? BirthDate { get; }

        public IdDocumentNumberDto IdDocumentNumber { get; }

        public static DuplicateSearchContext Create(Guid userId, IdDocumentNumberDto idDocumentNumber)
        {
            if (idDocumentNumber == null)
                throw new ArgumentNullException(nameof(idDocumentNumber));

            return new(userId, null, null, idDocumentNumber);
        }

        public static DuplicateSearchContext Create(Guid userId, FullNameDto fullName, DateTime birthDate)
        {
            if (fullName == null)
                throw new ArgumentNullException(nameof(fullName));

            return new(userId, fullName, birthDate, null);
        }
    }

    public class DuplicateSearchResult
    {
        private DuplicateSearchResult(DuplicateMatch[] matches, int total)
        {
            Matches = matches ?? throw new ArgumentNullException(nameof(matches));
            Total = total;
        }

        public DuplicateMatch[] Matches { get; }

        public int Total { get; }

        public static DuplicateSearchResult Empty => new (Array.Empty<DuplicateMatch>(), 0);

        public static DuplicateSearchResult Create(DuplicateMatch[] matches, int total) => new(matches, total);
    }

    public interface IDuplicateSearchService
    {
        Task<DuplicateSearchResult> FindAsync(DuplicateSearchContext context);
    }
}
