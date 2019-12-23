using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class MovedTypicalLifetime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TypicalLifetime",
                table: "Articles");

            migrationBuilder.AddColumn<int>(
                name: "TypicalLifetime",
                table: "ArticleUserSettings",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TypicalLifetime",
                table: "ArticleUserSettings");

            migrationBuilder.AddColumn<int>(
                name: "TypicalLifetime",
                table: "Articles",
                nullable: false,
                defaultValue: 0);
        }
    }
}
