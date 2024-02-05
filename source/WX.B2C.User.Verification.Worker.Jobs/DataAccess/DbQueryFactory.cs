using System;
using System.Security;
using Microsoft.Data.SqlClient;
using Serilog;
using SqlKata.Compilers;
using SqlKata.Execution;
using WX.B2C.User.Verification.Infrastructure.Contracts.Configuration;
using WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults;
using WX.Core.TypeExtensions;

namespace WX.B2C.User.Verification.Worker.Jobs.DataAccess
{
    public interface IQueryFactory
    {
        QueryFactory Create();
    }

    public abstract class QueryFactoryBase : IQueryFactory
    {
        private readonly SecureString _connectionString;
        private readonly ILogger _logger;

        protected QueryFactoryBase(SecureString connectionString, ILogger logger)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _logger = logger?.ForContext(GetType()) ?? throw new ArgumentNullException(nameof(logger));
        }

        public QueryFactory Create()
        {
            var dbConnection = new SqlConnection(_connectionString.UnSecure());
            var factory = new QueryFactory(dbConnection, new SqlServerCompiler { UseLegacyPagination = false });
            factory.Logger = result => _logger.Debug(result.ToString());
            return factory;
        }
    }
    
    /// <summary>
    /// Default query factory.
    /// </summary>
    public class B2CUserVerificationQueryFactory : QueryFactoryBase
    {
        public B2CUserVerificationQueryFactory(IAppConfig appConfig, ILogger logger) :
            base(appConfig.DbConnectionString, logger)
        {

        }
    }

    public interface IUserVerificationQueryFactory : IQueryFactory
    {

    }

    public class UserVerificationQueryFactory : QueryFactoryBase, IUserVerificationQueryFactory
    {
        public UserVerificationQueryFactory(IUserVerificationKeyVault keyVault, ILogger logger) :
            base(keyVault.DbConnectionString, logger)
        {

        }
    }


}