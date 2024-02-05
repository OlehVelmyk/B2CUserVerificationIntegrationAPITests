using System;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class NewVerificationApplicationDto
    {
        public ProductType ProductType { get; set; }

        public Guid PolicyId { get; set; }
    }
}
