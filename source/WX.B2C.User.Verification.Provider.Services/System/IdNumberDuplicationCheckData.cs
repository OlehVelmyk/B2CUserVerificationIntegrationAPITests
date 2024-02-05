using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Provider.Services.System
{
    public class IdNumberDuplicationInputData
    {
        public Guid UserId { get; set; }

        public IdDocumentNumberDto IdDocumentNumber { get; set; }
    }

    internal class IdNumberDuplicationOutputData : CheckOutputData
    {
        public IdNumberDuplicationFailReason Reason { get; set; }

        public DuplicateMatch[] Matches { get; set; }

        public int Total { get; set; }

        public static IdNumberDuplicationOutputData NotPresented() =>
            new() { Reason = IdNumberDuplicationFailReason.NotPresentedIdDocNumber };

        public static IdNumberDuplicationOutputData MatchFound(DuplicateMatch[] matches, int total)
        {
            if (matches is not { Length: > 0 })
                throw new ArgumentNullException(nameof(matches));
            if (total == 0)
                throw new ArgumentException(nameof(total));

            return new()
            {
                Reason = IdNumberDuplicationFailReason.DuplicationMatchFound,
                Matches = matches,
                Total = total
            };
        }
    }

    internal enum IdNumberDuplicationFailReason
    {
        NotPresentedIdDocNumber = 1,
        DuplicationMatchFound = 2
    }
}