using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using WX.B2C.User.Verification.BlobStorage.Storages;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults;
using WX.B2C.User.Verification.Worker.Jobs.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.Models.CollectionSteps;
using WX.B2C.User.Verification.Worker.Jobs.Services;
using WX.B2C.User.Verification.Worker.Jobs.Settings;
using WX.Core.TypeExtensions;

namespace WX.B2C.User.Verification.Worker.Jobs.Providers.CollectionSteps
{
    internal interface IDocumentsChecksProvider : IBatchJobDataProvider<DocumentChecks, CollectionStepsJobSettings> { }

    internal class DocumentsChecksProvider : IDocumentsChecksProvider
    {
        private readonly IUserVerificationKeyVault _userVerificationKeyVault;
        private readonly ICsvBlobStorage _csvBlobStorage;

        public DocumentsChecksProvider(IUserVerificationKeyVault userVerificationKeyVault, 
                                       ICsvBlobStorage csvBlobStorage)
        {
            _userVerificationKeyVault = userVerificationKeyVault ?? throw new ArgumentNullException(nameof(userVerificationKeyVault));
            _csvBlobStorage = csvBlobStorage ?? throw new ArgumentNullException(nameof(csvBlobStorage));
        }

        public async Task<int> GetTotalCountAsync(CollectionStepsJobSettings settings, CancellationToken cancellationToken)
        {
	        var users = await GetUsers(settings);
	        return users.Length;
        }

        public async IAsyncEnumerable<ICollection<DocumentChecks>> GetAsync(CollectionStepsJobSettings settings,
                                                                            [EnumeratorCancellation] CancellationToken cancellationToken)
        { 
            var users = await GetUsers(settings);
            var size = settings.ReadingBatchSize;
            var pageCount = (users.Length - 1) / size + 1;
            var connectionString = _userVerificationKeyVault.DbConnectionString.UnSecure();
            
            for (var page = 0; page < pageCount && !cancellationToken.IsCancellationRequested; page++)
            {
	            var usersInBatch = users.Skip(page * size).Take(size).ToArray();
	            var query = BuildQuery(usersInBatch);
	            await using var dbConnection = new SqlConnection(connectionString);
	            var batch = (await dbConnection.QueryAsync<DocumentChecks>(query)).ToArray();
	            yield return batch.ToArray();
            }
        }

        private string BuildQuery(Guid[] usersInBatch)
        {
	        var usersPredicate = usersInBatch.WhereIn("pi.ProfileInformationId");
	        var query = $@"
				select 
				pi.ProfileInformationId as UserId,
				pi.VerificationStatus as VerificationStatus,
				pi.VerificationStopReason,
				poa.Status as PoAStatus,
				pof.Status as PoFStatus
				from ProfileInformations as pi
				left join
					(select UserId, Status,
						 rowNumber = ROW_NUMBER() OVER (PARTITION BY UserId ORDER by CreatedAt DESC)
						 from ProofOfAddressChecks
					) as poa on poa.UserId = pi.ProfileInformationId AND poa.rowNumber = 1
				left join
					(select UserId, Status,
						 rowNumber = ROW_NUMBER() OVER (PARTITION BY UserId ORDER by CreatedAt DESC)
						 from ProofOfFundsChecks
					) as pof on pof.UserId = pi.ProfileInformationId AND pof.rowNumber = 1
                {usersPredicate}				
                ORDER BY pi.ProfileInformationId";

	        return query;
        }

        private async Task<Guid[]> GetUsers(CollectionStepsJobSettings settings)
        {
	        if (!settings.Users.IsNullOrEmpty())
		        return settings.Users;

	        var users = await _csvBlobStorage.GetAsync<Models.User>(settings.ContainerName, settings.FileName);
	        return users.Select(user => user.UserId).ToArray();
        }
    }
}