using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class RenameTypicalLifetime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TypicalLifetime",
                table: "ArticleUserSettings",
                newName: "UsualLifetime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UsualLifetime",
                table: "ArticleUserSettings",
                newName: "TypicalLifetime");
        }
    }
}
