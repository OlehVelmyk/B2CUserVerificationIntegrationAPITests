using System;

namespace WX.B2C.User.Verification.BlobStorage.Configurations
{
    public class BlobJsonConfiguration
    {
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
        public string BlobPath { get; set; }

        public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(20);

        /// <summary>
        /// TODO all below can be used later when options will be implemented
        /// </summary>
        public bool ReloadOnChange { get; set; }
        public Action<Exception> LogReloadException { get; set; }
        public Action OnReload { get; set; }
    }
}