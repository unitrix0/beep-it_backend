using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class AddStockEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManageEnvironments",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "AmountOnStock",
                table: "ArticleUserSettings");

            migrationBuilder.DropColumn(
                name: "IsOpened",
                table: "ArticleUserSettings");

            migrationBuilder.DropColumn(
                name: "OpenedOn",
                table: "ArticleUserSettings");

            migrationBuilder.DropColumn(
                name: "StockAmount",
                table: "ArticleUserSettings");

            migrationBuilder.CreateTable(
                name: "StockEntries",
                columns: table => new
                {
                    EnvironmentId = table.Column<int>(nullable: false),
                    ArticleId = table.Column<int>(nullable: false),
                    StockAmount = table.Column<int>(nullable: false),
                    IsOpened = table.Column<bool>(nullable: false),
                    AmountOnStock = table.Column<int>(nullable: false),
                    OpenedOn = table.Column<DateTime>(nullable: false),
                    ExpireDate = table.Column<DateTime>(nullable: false),
                    AmountRemaining = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockEntries", x => new { x.EnvironmentId, x.ArticleId });
                    table.ForeignKey(
                        name: "FK_StockEntries_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockEntries_Environments_EnvironmentId",
                        column: x => x.EnvironmentId,
                        principalTable: "Environments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockEntries_ArticleId",
                table: "StockEntries",
                column: "ArticleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockEntries");

            migrationBuilder.AddColumn<bool>(
                name: "ManageEnvironments",
                table: "Permissions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "AmountOnStock",
                table: "ArticleUserSettings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsOpened",
                table: "ArticleUserSettings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "OpenedOn",
                table: "ArticleUserSettings",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "StockAmount",
                table: "ArticleUserSettings",
                nullable: false,
                defaultValue: 0);
        }
    }
}
