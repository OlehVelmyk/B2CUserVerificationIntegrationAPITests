using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace WX.B2C.User.Verification.Component.Tests.Fixtures
{
    internal class DbFixture
    {
        private readonly string _connectionString;
        private readonly JsonSerializerSettings _typeNamedSerializerSettings;

        public DbFixture(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _typeNamedSerializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
        }

        public async Task<string> FindExternalId(Guid fileId)
        {
            using var queryFactory = CreateQueryFactory();
            var query = new Query("Files").Select("ExternalId").Where("Id", fileId);
            return await queryFactory.FirstOrDefaultAsync<string>(query);
        }
		
		public async Task<bool> ReminderExists(Guid userId, Guid targetId)
        {
            var query = new Query("Reminders").Where("UserId", userId).Where("TargetId", targetId);

            using var queryFactory = CreateQueryFactory();
            return await queryFactory.ExistsAsync(query);
        }

        private QueryFactory CreateQueryFactory()
        {
            var dbConnection = new SqlConnection(_connectionString);
            return new QueryFactory(dbConnection, new SqlServerCompiler { UseLegacyPagination = false });
        }

        private static string SerializeObject<T>(T data, JsonSerializerSettings settings = null)
        {
            if (data is null)
                return null;

            settings ??= new JsonSerializerSettings();

            return JsonConvert.SerializeObject(data, settings);
        }
    }
}
