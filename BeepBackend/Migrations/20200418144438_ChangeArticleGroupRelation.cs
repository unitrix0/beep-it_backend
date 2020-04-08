using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class ChangeArticleGroupRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleGroups_Environments_EnvironmentId",
                table: "ArticleGroups");

            migrationBuilder.RenameColumn(
                name: "EnvironmentId",
                table: "ArticleGroups",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ArticleGroups_EnvironmentId",
                table: "ArticleGroups",
                newName: "IX_ArticleGroups_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleGroups_AspNetUsers_UserId",
                table: "ArticleGroups",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleGroups_AspNetUsers_UserId",
                table: "ArticleGroups");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ArticleGroups",
                newName: "EnvironmentId");

            migrationBuilder.RenameIndex(
                name: "IX_ArticleGroups_UserId",
                table: "ArticleGroups",
                newName: "IX_ArticleGroups_EnvironmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleGroups_Environments_EnvironmentId",
                table: "ArticleGroups",
                column: "EnvironmentId",
                principalTable: "Environments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
