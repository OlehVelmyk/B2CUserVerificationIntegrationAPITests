using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Worker.Jobs.Settings
{
    internal class TaskStateJobSettings : CsvBlobJobSettings
    {
        public TaskState State { get; set; }

        public TaskResult? Result { get; set; }

        public bool UseActors { get; set; }
    }
}