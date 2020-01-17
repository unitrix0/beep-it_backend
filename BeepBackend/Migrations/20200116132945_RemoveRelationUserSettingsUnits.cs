using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class RemoveRelationUserSettingsUnits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleUserSettings_ArticleUnits_UnitId",
                table: "ArticleUserSettings");

            migrationBuilder.DropIndex(
                name: "IX_ArticleUserSettings_UnitId",
                table: "ArticleUserSettings");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "ArticleUserSettings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "ArticleUserSettings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ArticleUserSettings_UnitId",
                table: "ArticleUserSettings",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleUserSettings_ArticleUnits_UnitId",
                table: "ArticleUserSettings",
                column: "UnitId",
                principalTable: "ArticleUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
