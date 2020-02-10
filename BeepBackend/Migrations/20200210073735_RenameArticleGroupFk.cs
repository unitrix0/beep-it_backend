using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class RenameArticleGroupFk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_ArticleGroups_ArticleGroupFk",
                table: "Articles");

            migrationBuilder.RenameColumn(
                name: "ArticleGroupFk",
                table: "Articles",
                newName: "ArticleGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Articles_ArticleGroupFk",
                table: "Articles",
                newName: "IX_Articles_ArticleGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_ArticleGroups_ArticleGroupId",
                table: "Articles",
                column: "ArticleGroupId",
                principalTable: "ArticleGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_ArticleGroups_ArticleGroupId",
                table: "Articles");

            migrationBuilder.RenameColumn(
                name: "ArticleGroupId",
                table: "Articles",
                newName: "ArticleGroupFk");

            migrationBuilder.RenameIndex(
                name: "IX_Articles_ArticleGroupId",
                table: "Articles",
                newName: "IX_Articles_ArticleGroupFk");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_ArticleGroups_ArticleGroupFk",
                table: "Articles",
                column: "ArticleGroupFk",
                principalTable: "ArticleGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
