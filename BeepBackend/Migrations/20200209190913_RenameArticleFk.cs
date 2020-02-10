using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class RenameArticleFk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleUserSettings_Articles_ArticleFk",
                table: "ArticleUserSettings");

            migrationBuilder.RenameColumn(
                name: "ArticleFk",
                table: "ArticleUserSettings",
                newName: "ArticleId");

            migrationBuilder.RenameIndex(
                name: "IX_ArticleUserSettings_ArticleFk",
                table: "ArticleUserSettings",
                newName: "IX_ArticleUserSettings_ArticleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleUserSettings_Articles_ArticleId",
                table: "ArticleUserSettings",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleUserSettings_Articles_ArticleId",
                table: "ArticleUserSettings");

            migrationBuilder.RenameColumn(
                name: "ArticleId",
                table: "ArticleUserSettings",
                newName: "ArticleFk");

            migrationBuilder.RenameIndex(
                name: "IX_ArticleUserSettings_ArticleId",
                table: "ArticleUserSettings",
                newName: "IX_ArticleUserSettings_ArticleFk");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleUserSettings_Articles_ArticleFk",
                table: "ArticleUserSettings",
                column: "ArticleFk",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
