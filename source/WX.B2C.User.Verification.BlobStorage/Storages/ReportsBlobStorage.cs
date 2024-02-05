using System;
using System.IO;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;

namespace WX.B2C.User.Verification.BlobStorage.Storages
{
    internal class ReportsBlobStorage : IReportsBlobStorage
    {
        private readonly IBlobStorage _blobStorage;
        
        public ReportsBlobStorage(IBlobStorage blobStorage)
        {
            _blobStorage = blobStorage ?? throw new ArgumentNullException(nameof(blobStorage));
        }

        public Task AppendAsync(string containerName, string reportName, Stream reportPart) =>
            _blobStorage.AppendAsync(containerName, reportName, reportPart);

        public Task<string> GenerateDownloadHrefAsync(string containerName, string reportName, int expiryTime) =>
            _blobStorage.GenerateSasUriAsync(containerName, reportName, expiryTime);
    }
}