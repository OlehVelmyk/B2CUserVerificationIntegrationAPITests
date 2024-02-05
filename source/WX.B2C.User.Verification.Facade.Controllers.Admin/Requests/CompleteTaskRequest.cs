using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Requests
{
    public class CompleteTaskRequest
    {
        public TaskResult Result { get; set; }
        
        public string Reason { get; set; }
    }
}