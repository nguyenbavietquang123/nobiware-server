using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spark.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddStaticAdminRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "acb2878f-d9ab-4c78-be93-5a10c77ae8b1");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e84b6d63-36dc-4892-843e-648db78910a1", null, "Admin", "ADMIN" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e84b6d63-36dc-4892-843e-648db78910a1");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "acb2878f-d9ab-4c78-be93-5a10c77ae8b1", null, "Admin", "ADMIN" });
        }
    }
}
