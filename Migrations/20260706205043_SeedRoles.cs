using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GameLibraryAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "b8b67401-5ff9-4bd8-9242-dfed31756ef5", "90501afe-ff58-42f5-b53d-6d6874c09d63", "Admin", "ADMIN" },
                    { "c14c7310-8692-4c33-85fb-3a9f778eefb0", "058773e7-8958-42a0-80de-22206f592b1d", "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b8b67401-5ff9-4bd8-9242-dfed31756ef5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c14c7310-8692-4c33-85fb-3a9f778eefb0");
        }
    }
}
