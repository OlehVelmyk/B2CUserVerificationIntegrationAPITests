using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WX.B2C.User.Verification.DataAccess.Migrations
{
    public partial class UpdateValidationPolicies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("0735076b-99d2-4f9f-a82d-370cf9456b12"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("0735076b-99d2-4f9f-a82d-370cf9456b12"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("150159b9-1656-4c85-903f-8f90e4cc4acc"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("150159b9-1656-4c85-903f-8f90e4cc4acc"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("24adc526-8cf1-44e3-8980-5f845411d5f3"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("24adc526-8cf1-44e3-8980-5f845411d5f3"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("4e18c9e3-a215-4527-b626-df1b9f7bdab2"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("4e18c9e3-a215-4527-b626-df1b9f7bdab2"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("69233213-ec1c-49e2-ae25-a9bd3016fbed"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("69233213-ec1c-49e2-ae25-a9bd3016fbed"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("79d98c07-27ca-4168-8434-ef536b40c8de"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("79d98c07-27ca-4168-8434-ef536b40c8de"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("84d8f664-332a-4880-b942-ef1fa2707622"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("84d8f664-332a-4880-b942-ef1fa2707622"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("a0b5d0ad-2c34-413a-87ec-91c2d343b994"), new Guid("5cbecb0b-86d7-4aa3-9361-5a0414200910") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("a0b5d0ad-2c34-413a-87ec-91c2d343b994"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("a0b5d0ad-2c34-413a-87ec-91c2d343b994"), new Guid("b512e3ad-764e-4e78-83cc-d466354746be") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("dcfe96d5-326e-4489-850f-03af13388200"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("dcfe96d5-326e-4489-850f-03af13388200"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("e691c2f3-cde4-40aa-93bd-9f9b8a867b27"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("e691c2f3-cde4-40aa-93bd-9f9b8a867b27"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("ee49481d-b3d6-47b0-afa2-db4de028ef97"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("ee49481d-b3d6-47b0-afa2-db4de028ef97"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("f19aa3f4-9d10-4fe2-8d11-214b1de1027b"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("f19aa3f4-9d10-4fe2-8d11-214b1de1027b"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("f20795f5-36ca-4209-a20b-2ddac7563f9a"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("f20795f5-36ca-4209-a20b-2ddac7563f9a"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") });

            migrationBuilder.InsertData(
                schema: "Policy",
                table: "PolicyValidationRules",
                columns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                values: new object[] { new Guid("b886b233-9e03-4afe-a0aa-4580adc01ef1"), new Guid("5cbecb0b-86d7-4aa3-9361-5a0414200910") });

            migrationBuilder.InsertData(
                schema: "Policy",
                table: "PolicyValidationRules",
                columns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                values: new object[] { new Guid("b886b233-9e03-4afe-a0aa-4580adc01ef1"), new Guid("b512e3ad-764e-4e78-83cc-d466354746be") });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("b886b233-9e03-4afe-a0aa-4580adc01ef1"), new Guid("5cbecb0b-86d7-4aa3-9361-5a0414200910") });

            migrationBuilder.DeleteData(
                schema: "Policy",
                table: "PolicyValidationRules",
                keyColumns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                keyValues: new object[] { new Guid("b886b233-9e03-4afe-a0aa-4580adc01ef1"), new Guid("b512e3ad-764e-4e78-83cc-d466354746be") });

            migrationBuilder.InsertData(
                schema: "Policy",
                table: "PolicyValidationRules",
                columns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                values: new object[,]
                {
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("150159b9-1656-4c85-903f-8f90e4cc4acc"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") },
                    { new Guid("150159b9-1656-4c85-903f-8f90e4cc4acc"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("e691c2f3-cde4-40aa-93bd-9f9b8a867b27"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") },
                    { new Guid("e691c2f3-cde4-40aa-93bd-9f9b8a867b27"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("dcfe96d5-326e-4489-850f-03af13388200"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") },
                    { new Guid("dcfe96d5-326e-4489-850f-03af13388200"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("69233213-ec1c-49e2-ae25-a9bd3016fbed"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") },
                    { new Guid("69233213-ec1c-49e2-ae25-a9bd3016fbed"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("f20795f5-36ca-4209-a20b-2ddac7563f9a"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") },
                    { new Guid("f20795f5-36ca-4209-a20b-2ddac7563f9a"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("f19aa3f4-9d10-4fe2-8d11-214b1de1027b"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") },
                    { new Guid("f19aa3f4-9d10-4fe2-8d11-214b1de1027b"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("84d8f664-332a-4880-b942-ef1fa2707622"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") },
                    { new Guid("79d98c07-27ca-4168-8434-ef536b40c8de"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("84d8f664-332a-4880-b942-ef1fa2707622"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("ee49481d-b3d6-47b0-afa2-db4de028ef97"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("a0b5d0ad-2c34-413a-87ec-91c2d343b994"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("a0b5d0ad-2c34-413a-87ec-91c2d343b994"), new Guid("b512e3ad-764e-4e78-83cc-d466354746be") },
                    { new Guid("a0b5d0ad-2c34-413a-87ec-91c2d343b994"), new Guid("5cbecb0b-86d7-4aa3-9361-5a0414200910") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("4e18c9e3-a215-4527-b626-df1b9f7bdab2"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") },
                    { new Guid("4e18c9e3-a215-4527-b626-df1b9f7bdab2"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("0735076b-99d2-4f9f-a82d-370cf9456b12"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") },
                    { new Guid("0735076b-99d2-4f9f-a82d-370cf9456b12"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("24adc526-8cf1-44e3-8980-5f845411d5f3"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") },
                    { new Guid("24adc526-8cf1-44e3-8980-5f845411d5f3"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") },
                    { new Guid("ee49481d-b3d6-47b0-afa2-db4de028ef97"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") },
                    { new Guid("79d98c07-27ca-4168-8434-ef536b40c8de"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") }
                });
        }
    }
}
