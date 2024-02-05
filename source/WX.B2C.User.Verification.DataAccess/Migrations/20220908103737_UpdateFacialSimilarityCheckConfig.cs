using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WX.B2C.User.Verification.DataAccess.Migrations
{
    public partial class UpdateFacialSimilarityCheckConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "ChecksVariants",
                keyColumn: "Id",
                keyValue: new Guid("23714f13-cbf6-41a4-85c6-719991e6c3f3"),
                column: "Config",
                value: "{\"reportName\":\"facial_similarity_video\",\"isSelfieOptional\":true,\"isVideoRequired\":true}");

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "ChecksVariants",
                keyColumn: "Id",
                keyValue: new Guid("29aac87b-3ad4-40e0-b34f-3685ca64805d"),
                column: "Config",
                value: "{\"reportName\":\"facial_similarity_photo\",\"isSelfieOptional\":true,\"isVideoRequired\":false}");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "ChecksVariants",
                keyColumn: "Id",
                keyValue: new Guid("23714f13-cbf6-41a4-85c6-719991e6c3f3"),
                column: "Config",
                value: "{\"reportName\":\"facial_similarity_video\",\"isVideoRequired\":true}");

            migrationBuilder.UpdateData(
                schema: "Policy",
                table: "ChecksVariants",
                keyColumn: "Id",
                keyValue: new Guid("29aac87b-3ad4-40e0-b34f-3685ca64805d"),
                column: "Config",
                value: "{\"reportName\":\"facial_similarity_photo\",\"isVideoRequired\":false}");
        }
    }
}
