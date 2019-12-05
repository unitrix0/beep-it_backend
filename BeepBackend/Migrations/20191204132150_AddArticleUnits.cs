using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class AddArticleUnits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleUserSettings_Environments_EnvironmentId",
                table: "ArticleUserSettings");

            migrationBuilder.AlterColumn<int>(
                name: "EnvironmentId",
                table: "ArticleUserSettings",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "ArticleUserSettings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "Articles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ArticleUnits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Abbreviation = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleUnits", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ArticleUnits",
                columns: new[] { "Id", "Abbreviation", "Name" },
                values: new object[,]
                {
                    { 1, "Stk.", "Stück" },
                    { 2, "l", "Liter" },
                    { 3, "dl", "Deziliter" },
                    { 4, "cl", "Centiliter" },
                    { 5, "g", "Gramm" },
                    { 6, "kg", "Kilogramm" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleUserSettings_UnitId",
                table: "ArticleUserSettings",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_UnitId",
                table: "Articles",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_ArticleUnits_UnitId",
                table: "Articles",
                column: "UnitId",
                principalTable: "ArticleUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleUserSettings_Environments_EnvironmentId",
                table: "ArticleUserSettings",
                column: "EnvironmentId",
                principalTable: "Environments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleUserSettings_ArticleUnits_UnitId",
                table: "ArticleUserSettings",
                column: "UnitId",
                principalTable: "ArticleUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_ArticleUnits_UnitId",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK_ArticleUserSettings_Environments_EnvironmentId",
                table: "ArticleUserSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_ArticleUserSettings_ArticleUnits_UnitId",
                table: "ArticleUserSettings");

            migrationBuilder.DropTable(
                name: "ArticleUnits");

            migrationBuilder.DropIndex(
                name: "IX_ArticleUserSettings_UnitId",
                table: "ArticleUserSettings");

            migrationBuilder.DropIndex(
                name: "IX_Articles_UnitId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "ArticleUserSettings");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "Articles");

            migrationBuilder.AlterColumn<int>(
                name: "EnvironmentId",
                table: "ArticleUserSettings",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleUserSettings_Environments_EnvironmentId",
                table: "ArticleUserSettings",
                column: "EnvironmentId",
                principalTable: "Environments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
