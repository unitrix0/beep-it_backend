using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class AddArticleUserSettingsEnvironmentRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EnvironmentFk",
                table: "ArticleUserSettings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ArticleUserSettings_EnvironmentFk",
                table: "ArticleUserSettings",
                column: "EnvironmentFk");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleUserSettings_Environments_EnvironmentFk",
                table: "ArticleUserSettings",
                column: "EnvironmentFk",
                principalTable: "Environments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleUserSettings_Environments_EnvironmentFk",
                table: "ArticleUserSettings");

            migrationBuilder.DropIndex(
                name: "IX_ArticleUserSettings_EnvironmentFk",
                table: "ArticleUserSettings");

            migrationBuilder.DropColumn(
                name: "EnvironmentFk",
                table: "ArticleUserSettings");
        }
    }
}
