using System.IO;
using System.Threading.Tasks;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface IReportsBlobStorage
    {
        Task AppendAsync(string containerName, string reportName, Stream reportPart);

        Task<string> GenerateDownloadHrefAsync(string containerName, string reportName, int expiryTime);
    }
}