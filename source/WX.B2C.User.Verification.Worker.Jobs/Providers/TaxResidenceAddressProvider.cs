using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults;
using WX.B2C.User.Verification.Worker.Jobs.Models;
using WX.B2C.User.Verification.Worker.Jobs.Settings;

namespace WX.B2C.User.Verification.Worker.Jobs.Providers
{
    internal interface ITaxResidenceAddressProvider
    {
        public Task<ICollection<TaxResidenceAddressData>> GetAsync(TaxResidenceJobSetting settings, Guid[] usersIds);
    }

    internal class TaxResidenceAddressProvider : ITaxResidenceAddressProvider
    {
        private readonly IAppConfig _appConfig;

        public TaxResidenceAddressProvider(IAppConfig appConfig, IUserVerificationKeyVault userVerificationKeyVault)
        {
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
        }

        public async Task<ICollection<TaxResidenceAddressData>> GetAsync(TaxResidenceJobSetting settings, Guid[] usersIds)
        {
            throw new NotImplementedException("Must not be used at all. Should be read with main provider");
            ////var connectionString = _appConfig.DbConnectionString.UnSecure();

            ////var query = QueryBuilder.Select("ResidenceAddresses",
            ////                                "UserId, Country")
            ////                        .WhereIn("UserId", usersIds)
            ////                        .Build();

            ////await using var dbConnection = new SqlConnection(connectionString);
            ////var batch = (await dbConnection.QueryAsync<TaxResidenceAddressData>(query)).ToArray();
            ////await dbConnection.CloseAsync();
            ////return batch;
        }
    }
}