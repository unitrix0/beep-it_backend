using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class AddRelationEnvironmentArtivleUserSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EnvironmentId",
                table: "ArticleUserSettings",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArticleUserSettings_EnvironmentId",
                table: "ArticleUserSettings",
                column: "EnvironmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleUserSettings_Environments_EnvironmentId",
                table: "ArticleUserSettings",
                column: "EnvironmentId",
                principalTable: "Environments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleUserSettings_Environments_EnvironmentId",
                table: "ArticleUserSettings");

            migrationBuilder.DropIndex(
                name: "IX_ArticleUserSettings_EnvironmentId",
                table: "ArticleUserSettings");

            migrationBuilder.DropColumn(
                name: "EnvironmentId",
                table: "ArticleUserSettings");
        }
    }
}
