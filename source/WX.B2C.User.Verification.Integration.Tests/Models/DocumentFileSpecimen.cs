using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Integration.Tests.Models
{
    internal class DocumentFileSpecimen
    {
        public Guid Id { get; set; }

        public FileDto File { get; set; }
    }
}
