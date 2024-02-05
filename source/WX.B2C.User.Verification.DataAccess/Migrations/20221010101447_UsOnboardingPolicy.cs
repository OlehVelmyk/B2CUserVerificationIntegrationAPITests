using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WX.B2C.User.Verification.DataAccess.Migrations
{
    public partial class UsOnboardingPolicy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "ChecksVariants",
                keyColumn: "Id",
                keyValue: new Guid("34a33df0-b9b5-4205-9cc6-1f90be10d313"),
                column: "Config",
                value: "{\"searchNames\":[\"Political Exposure\",\"Reputational Risk\",\"Sanctions Screening\"]}");

            migrationBuilder.InsertData(
                schema: "Policy",
                table: "ChecksVariants",
                columns: new[] { "Id", "Config", "FailResult", "FailResultCondition", "FailResultType", "Name", "Provider", "RunPolicy", "Type" },
                values: new object[] { new Guid("3d410715-dfc6-4952-82ca-9fd1ad53cba5"), "{\"searchNames\":[\"PEP Family Members\"]}", null, null, null, "US Pep Family Members", "LexisNexis", null, "RiskListsScreening" });

            migrationBuilder.InsertData(
                schema: "Policy",
                table: "Monitoring",
                columns: new[] { "Id", "Name", "Region", "RegionType" },
                values: new object[] { new Guid("fff1401f-6888-4c05-ba23-4843e8e88ec9"), "USA monitoring policy", "US", 3 });

            migrationBuilder.InsertData(
                schema: "Policy",
                table: "Triggers",
                columns: new[] { "Id", "Commands", "Conditions", "Iterative", "Name", "PolicyId", "Preconditions", "Schedule" },
                values: new object[] { new Guid("50283357-6227-4793-b104-5331c80f86e2"), "[{\"Type\":3,\"Config\":\"{\\\"taskType\\\":\\\"RiskListsScreening\\\",\\\"variantId\\\":\\\"3D410715-DFC6-4952-82CA-9FD1AD53CBA5\\\",\\\"force\\\":false}\"}]", "[{\"Type\":4,\"Value\":{\"riskLevel\":\"ExtraHigh\",\"threshold\":400}},{\"Type\":4,\"Value\":{\"riskLevel\":\"High\",\"threshold\":400}},{\"Type\":4,\"Value\":{\"riskLevel\":\"Medium\",\"threshold\":400}},{\"Type\":4,\"Value\":{\"riskLevel\":\"Low\",\"threshold\":400}}]", false, "PEP Family Members", new Guid("fff1401f-6888-4c05-ba23-4843e8e88ec9"), null, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "ChecksVariants",
                keyColumn: "Id",
                keyValue: new Guid("3d410715-dfc6-4952-82ca-9fd1ad53cba5"));

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "Monitoring",
                keyColumn: "Id",
                keyValue: new Guid("fff1401f-6888-4c05-ba23-4843e8e88ec9"));

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "Triggers",
                keyColumn: "Id",
                keyValue: new Guid("50283357-6227-4793-b104-5331c80f86e2"));

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "ChecksVariants",
                keyColumn: "Id",
                keyValue: new Guid("34a33df0-b9b5-4205-9cc6-1f90be10d313"),
                column: "Config",
                value: null);
        }
    }
}
