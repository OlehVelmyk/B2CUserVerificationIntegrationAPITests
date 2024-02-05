using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WX.B2C.User.Verification.DataAccess.Migrations
{
    public partial class AddRepeatingTurnoverThresholdForExtraHighRiskLevelUsersForGgAndEeaPolicies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Triggers",
                keyColumn: "Id",
                keyValue: new Guid("6f762b87-112e-43c5-b311-c32cdf723d63"),
                column: "Conditions",
                value: "[{\"Type\":5,\"Value\":{\"riskLevel\":\"Low\",\"threshold\":75000,\"step\":75000}},{\"Type\":5,\"Value\":{\"riskLevel\":\"Medium\",\"threshold\":60000,\"step\":60000}},{\"Type\":5,\"Value\":{\"riskLevel\":\"High\",\"threshold\":15000,\"step\":30000}},{\"Type\":5,\"Value\":{\"riskLevel\":\"ExtraHigh\",\"threshold\":15000,\"step\":30000}}]");

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Triggers",
                keyColumn: "Id",
                keyValue: new Guid("e1ffb995-72cb-429e-99b2-db4282da6526"),
                column: "Conditions",
                value: "[{\"Type\":5,\"Value\":{\"riskLevel\":\"Low\",\"threshold\":75000,\"step\":75000}},{\"Type\":5,\"Value\":{\"riskLevel\":\"Medium\",\"threshold\":60000,\"step\":60000}},{\"Type\":5,\"Value\":{\"riskLevel\":\"High\",\"threshold\":15000,\"step\":30000}},{\"Type\":5,\"Value\":{\"riskLevel\":\"ExtraHigh\",\"threshold\":15000,\"step\":30000}}]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Triggers",
                keyColumn: "Id",
                keyValue: new Guid("6f762b87-112e-43c5-b311-c32cdf723d63"),
                column: "Conditions",
                value: "[{\"Type\":5,\"Value\":{\"riskLevel\":\"Low\",\"threshold\":75000,\"step\":75000}},{\"Type\":5,\"Value\":{\"riskLevel\":\"Medium\",\"threshold\":60000,\"step\":60000}},{\"Type\":5,\"Value\":{\"riskLevel\":\"High\",\"threshold\":15000,\"step\":30000}}]");

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Triggers",
                keyColumn: "Id",
                keyValue: new Guid("e1ffb995-72cb-429e-99b2-db4282da6526"),
                column: "Conditions",
                value: "[{\"Type\":5,\"Value\":{\"riskLevel\":\"Low\",\"threshold\":75000,\"step\":75000}},{\"Type\":5,\"Value\":{\"riskLevel\":\"Medium\",\"threshold\":60000,\"step\":60000}},{\"Type\":5,\"Value\":{\"riskLevel\":\"High\",\"threshold\":15000,\"step\":30000}}]");
        }
    }
}
