using System;

namespace WX.B2C.User.Verification.Worker.Jobs.Settings
{
    public class JobSettings
    {
    }

    public class BatchJobSettings : JobSettings
    {
        public int ReadingBatchSize { get; set; }

        public int ProcessBatchSize { get; set; }

        public int MaxErrorCount { get; set; }

        public int DelayInMillisecondsAfterBatch { get; set; }

        public int ProcessingBatchOffset { get; set; } = 0;
    }

    public class UserBatchJobSettings : BatchJobSettings, IEntityProvidedSettings
    {
        public Guid[] Users { get; set; }

        public Guid[] Ids => Users;
    }

    public interface IEntityProvidedSettings
    {
        Guid[] Ids { get; }
    }
}