using System;
using Microsoft.EntityFrameworkCore.Migrations;
using WX.EntityFrameworkCore.TemporalTables.Sql.Factory;

namespace WX.B2C.User.Verification.DataAccess.EF.Extensions
{
    internal static class MigrationBuilderExtensions
    {
        public static void CreateTemporalTable(this MigrationBuilder migrationBuilder, string tableName, string schema = "dbo")
        {
            if (migrationBuilder == null)
                throw new ArgumentNullException(nameof(migrationBuilder));

            var temporalTableSqlGenerator = new TemporalTableSqlGeneratorFactory()
                .CreateInstance(true, false, tableName, schema);

            migrationBuilder.Sql(temporalTableSqlGenerator.Generate());
        }

        public static void DropTemporalTable(this MigrationBuilder migrationBuilder, string tableName, string schema = "dbo")
        {
            if (migrationBuilder == null)
                throw new ArgumentNullException(nameof(migrationBuilder));

            var temporalTableSqlGenerator = new TemporalTableSqlGeneratorFactory()
                .CreateInstance(false, true, tableName, schema);

            migrationBuilder.Sql(temporalTableSqlGenerator.Generate());
        }
    }
}
