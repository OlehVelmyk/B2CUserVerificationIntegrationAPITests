using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Integration.Tests.Builders
{
    internal class TaskStateJobSettingsBuilder
    {
        private int _processBatchSize = 1;
        private int _readingBatchSize = 1;
        private int _delayInMillisecondsAfterBatch = 0;
        private int _maxErrorCount = 0;
        private string _fileName = string.Empty;
        private bool _useActors;
        private TaskState _taskState = TaskState.Incomplete;
        private TaskResult? _taskResult = null;

        public TaskStateJobSettingsBuilder WithTaskState(TaskState taskState)
        {
            _taskState = taskState;
            return this;
        }

        public TaskStateJobSettingsBuilder WithTaskResult(TaskResult result)
        {
            _taskResult = result;
            return this;
        }

        public TaskStateJobSettingsBuilder UseActors(bool useActors)
        {
            _useActors = useActors;
            return this;
        }

        public TaskStateJobSettings Build() =>
            new()
            {
                ProcessBatchSize = _processBatchSize,
                ReadingBatchSize = _readingBatchSize,
                DelayInMillisecondsAfterBatch = _delayInMillisecondsAfterBatch,
                MaxErrorCount = _maxErrorCount,
                FileName = _fileName,
                State = _taskState,
                Result = _taskResult,
                UseActors = _useActors
            };
    }
}