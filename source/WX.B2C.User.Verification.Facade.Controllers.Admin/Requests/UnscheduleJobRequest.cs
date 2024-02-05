using System.Collections.Generic;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Requests
{
    public class UnscheduleJobRequest
    {
        public string JobName { get; set; }

        public Dictionary<string, object> JobParameters { get; set; }
    }
}