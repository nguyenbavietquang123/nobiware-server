using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spark.Web.Migrations
{
    /// <inheritdoc />
    public partial class FixAdminSeedRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "453e564a-29b5-4f1f-b49b-1bd116778fce");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "acb2878f-d9ab-4c78-be93-5a10c77ae8b1", null, "Admin", "ADMIN" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "acb2878f-d9ab-4c78-be93-5a10c77ae8b1");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "453e564a-29b5-4f1f-b49b-1bd116778fce", "38c23849-bc6b-451e-bba0-c817703d3168", "Admin", "ADMIN" });
        }
    }
}
