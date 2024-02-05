using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace WX.B2C.User.Verification.BlobStorage.Factories
{
    internal interface IBlobContainerClientFactory
    {
        Task<BlobContainerClient> CreateAsync(string connectionString, string rootContainer);
    }

    internal class BlobContainerClientFactory : IBlobContainerClientFactory
    {
        public async Task<BlobContainerClient> CreateAsync(string connectionString, string rootContainer)
        {
            var blobServiceClient = new BlobServiceClient(connectionString);

            // Not registering BlobContainerClient because Action Configuration already registers it with different RootContainer
            var isContainerCreated = await IsAlreadyCreatedAsync(rootContainer, blobServiceClient);

            if (isContainerCreated)
                return blobServiceClient.GetBlobContainerClient(rootContainer);

            return await blobServiceClient.CreateBlobContainerAsync(rootContainer);
        }

        private static async Task<bool> IsAlreadyCreatedAsync(string rootContainer, BlobServiceClient blobServiceClient)
        {
            var blobContainers =
                blobServiceClient.GetBlobContainersAsync(BlobContainerTraits.Metadata, rootContainer, CancellationToken.None);
            await foreach (var container in blobContainers)
            {
                if (container.Name == rootContainer)
                    return true;
            }
            return false;
        }
    }
}