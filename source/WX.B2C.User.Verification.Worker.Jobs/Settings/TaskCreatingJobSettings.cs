using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Worker.Jobs.Settings
{
    internal class TaskCreatingJobSettings : CsvBlobJobSettings
    {
        public TaskType[] TaskTypes { get; set; }
    }
}
