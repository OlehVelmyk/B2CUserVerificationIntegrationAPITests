using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WX.B2C.User.Verification.DataAccess.Migrations
{
    public partial class UpdateValidationRules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "Policy",
                table: "PolicyValidationRules",
                columns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                values: new object[] { new Guid("84d8f664-332a-4880-b942-ef1fa2707622"), new Guid("aa8c8f46-aaf5-4ea5-bbb2-f8f811da1f5c") });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("84d8f664-332a-4880-b942-ef1fa2707622"), new Guid("aa8c8f46-aaf5-4ea5-bbb2-f8f811da1f5c") });
        }
    }
}
