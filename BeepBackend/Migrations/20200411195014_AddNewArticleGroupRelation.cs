using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class AddNewArticleGroupRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_ArticleGroups_ArticleGroupId",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_ArticleGroupId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "ArticleGroupId",
                table: "Articles");

            migrationBuilder.AddColumn<int>(
                name: "ArticleGroupId",
                table: "ArticleUserSettings",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "EnvironmentId",
                table: "ArticleGroups",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArticleUserSettings_ArticleGroupId",
                table: "ArticleUserSettings",
                column: "ArticleGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleGroups_EnvironmentId",
                table: "ArticleGroups",
                column: "EnvironmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleGroups_Environments_EnvironmentId",
                table: "ArticleGroups",
                column: "EnvironmentId",
                principalTable: "Environments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleUserSettings_ArticleGroups_ArticleGroupId",
                table: "ArticleUserSettings",
                column: "ArticleGroupId",
                principalTable: "ArticleGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleGroups_Environments_EnvironmentId",
                table: "ArticleGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_ArticleUserSettings_ArticleGroups_ArticleGroupId",
                table: "ArticleUserSettings");

            migrationBuilder.DropIndex(
                name: "IX_ArticleUserSettings_ArticleGroupId",
                table: "ArticleUserSettings");

            migrationBuilder.DropIndex(
                name: "IX_ArticleGroups_EnvironmentId",
                table: "ArticleGroups");

            migrationBuilder.DropColumn(
                name: "ArticleGroupId",
                table: "ArticleUserSettings");

            migrationBuilder.DropColumn(
                name: "EnvironmentId",
                table: "ArticleGroups");

            migrationBuilder.AddColumn<int>(
                name: "ArticleGroupId",
                table: "Articles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_ArticleGroupId",
                table: "Articles",
                column: "ArticleGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_ArticleGroups_ArticleGroupId",
                table: "Articles",
                column: "ArticleGroupId",
                principalTable: "ArticleGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
