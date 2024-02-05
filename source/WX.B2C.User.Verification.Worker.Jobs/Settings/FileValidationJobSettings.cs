﻿using System;

namespace WX.B2C.User.Verification.Worker.Jobs.Settings
{
    internal class FileValidationJobSettings : UserBatchJobSettings
    {
        public Guid[] Files { get; set; }

        public int OnfidoRequestPerMinute { get; set; } = 100;
    }
}