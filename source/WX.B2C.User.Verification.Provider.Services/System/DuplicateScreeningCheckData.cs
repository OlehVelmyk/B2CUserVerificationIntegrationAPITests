using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Provider.Services.System
{
    public class DuplicateScreeningInputData
    {
        public Guid UserId { get; set; }

        public FullNameDto FullName { get; set; }

        public DateTime BirthDate { get; set; }
    }

    internal class DuplicateScreeningOutputData : CheckOutputData
    {
        public DuplicateMatch[] Matches { get; set; }

        public int Total { get; set; }
    }
}
