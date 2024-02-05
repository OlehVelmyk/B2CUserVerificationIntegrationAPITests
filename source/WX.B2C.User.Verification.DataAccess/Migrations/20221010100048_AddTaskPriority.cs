using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WX.B2C.User.Verification.DataAccess.Migrations
{
    public partial class AddTaskPriority : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Priority",
                schema: "Policy",
                table: "Tasks",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("02f9e694-49f2-4669-a5c8-bea234d92e03"),
                column: "Priority",
                value: 2);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("1673241e-bc0a-4007-a353-a2c39880bbef"),
                column: "Priority",
                value: 4);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("1dd5ae17-d87c-4b9f-b61f-a084b29abb4a"),
                column: "Priority",
                value: 3);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("2661697c-e75a-4b87-96cb-3261e9a460d7"),
                column: "Priority",
                value: 1);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("2772ddb3-1d85-4625-8479-67677d9622be"),
                column: "Priority",
                value: 3);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("2796b0cb-42cb-49b7-a30e-84824d603799"),
                column: "Priority",
                value: 2);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("28016523-2197-4c87-adc8-86bfa48a68fd"),
                column: "Priority",
                value: 1);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("2f2e1614-b199-4d7a-a8f0-e9aa810e29ec"),
                column: "Priority",
                value: 2);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("325704db-6db2-4dab-9c07-ef700e341dbe"),
                column: "Priority",
                value: 1);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("3b349c17-154a-4dc9-a683-045aa985836d"),
                column: "Priority",
                value: 2);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("41f23381-f8bb-42c7-8b58-c344a4ad011e"),
                column: "Priority",
                value: 4);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("543ec1a7-6141-4491-9ca6-3691bbddb7ee"),
                column: "Priority",
                value: 2);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("563af1e6-dc53-40a7-8604-78945f88b6a3"),
                column: "Priority",
                value: 1);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("62ba1278-c7f7-42d1-a901-0548fdb1a4db"),
                column: "Priority",
                value: 1);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("689c42ce-e4ac-4712-96a6-c2fcda404283"),
                column: "Priority",
                value: 1);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("72df5230-bb5a-4857-bcaf-9e27f0cd8a36"),
                column: "Priority",
                value: 1);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("782bdf03-5c44-45ad-a267-8a26934066a8"),
                column: "Priority",
                value: 2);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("7fc98096-1cb1-47fb-91bd-1f600beb82a0"),
                column: "Priority",
                value: 2);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("805be539-a68e-4e21-9b9e-b9f16dd91c8b"),
                column: "Priority",
                value: 2);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("828c3d69-d9b1-46d5-9498-45f3dd74b278"),
                column: "Priority",
                value: 2);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("886609cb-ed63-4aef-aee9-b51b75c2a829"),
                column: "Priority",
                value: 3);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("91ddffdb-4788-4783-a004-025b2357a9ed"),
                column: "Priority",
                value: 1);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("94538731-6f81-48c7-8664-13e70238e5c3"),
                column: "Priority",
                value: 1);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("9486f8ef-1730-45a7-a724-434d4e89c7c1"),
                column: "Priority",
                value: 1);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("9a1c60fb-6f96-431b-abdb-11b5fc9c5ca5"),
                column: "Priority",
                value: 3);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("9e202236-42b4-4105-a6a4-1356a82911a2"),
                column: "Priority",
                value: 4);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("a8d1ca2b-1abb-49d7-a5ae-74e5dd3d9f5e"),
                column: "Priority",
                value: 2);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("aa49652a-74c8-4667-a20f-a92fe59cbf2b"),
                column: "Priority",
                value: 3);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("b5160ff6-c9e6-4492-9422-96eb6b8f42ef"),
                column: "Priority",
                value: 2);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("bc3341a5-e1b3-4f72-93a3-1a5324a92ff9"),
                column: "Priority",
                value: 2);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("c1b826b7-6e0a-4ab3-813d-4393e0c0e095"),
                column: "Priority",
                value: 4);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("c2badff2-b73f-4fb0-b447-a3eb48964b36"),
                column: "Priority",
                value: 4);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("c2d32093-ad0f-45ad-9377-4dd12550a221"),
                column: "Priority",
                value: 4);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("caefe21a-e362-4ff5-93b5-943158102c31"),
                column: "Priority",
                value: 1);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("cbeed7f8-943b-482f-81a5-4648f4f1fa04"),
                column: "Priority",
                value: 1);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("cf3d8afc-cbb4-4531-bea5-ddf2eb990cfe"),
                column: "Priority",
                value: 3);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("cf9f0ca8-6535-4f4a-b2d3-71ab075ec841"),
                column: "Priority",
                value: 4);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("d06a5360-bd42-490e-82eb-c910ba66acff"),
                column: "Priority",
                value: 1);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("d4d1e3d0-8fb8-46a8-a5a8-bfae7cf9e471"),
                column: "Priority",
                value: 1);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("f8743f21-a511-45a3-ab03-e833ab639afa"),
                column: "Priority",
                value: 2);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("fae04e22-af37-46a6-875a-c93a2ea8c9a3"),
                column: "Priority",
                value: 3);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("fbca7efc-39f1-4d26-8e5c-4feb4a7fd34f"),
                column: "Priority",
                value: 2);

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("fd2abd3f-3348-431c-a856-1a37f650398c"),
                column: "Priority",
                value: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                schema: "Policy",
                table: "Tasks");
        }
    }
}
