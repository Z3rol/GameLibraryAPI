using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameLibraryAPI.Migrations
{
    /// <inheritdoc />
    public partial class ChangeGameModelVariableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReleasedOn",
                table: "Games",
                newName: "ReleaseDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReleaseDate",
                table: "Games",
                newName: "ReleasedOn");
        }
    }
}
