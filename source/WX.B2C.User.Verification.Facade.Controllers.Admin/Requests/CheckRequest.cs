using System;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Requests
{
    public class CheckRequest
    {
        public Guid VariantId { get; set; }

        public Guid[] RelatedTasks { get; set; }

        public string Reason { get; set; }
    }
}