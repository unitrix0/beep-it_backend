using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class RenameArticelStores : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleStore_Articles_ArticleId",
                table: "ArticleStore");

            migrationBuilder.DropForeignKey(
                name: "FK_ArticleStore_Stores_StoreId",
                table: "ArticleStore");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ArticleStore",
                table: "ArticleStore");

            migrationBuilder.RenameTable(
                name: "ArticleStore",
                newName: "ArticleStores");

            migrationBuilder.RenameIndex(
                name: "IX_ArticleStore_StoreId",
                table: "ArticleStores",
                newName: "IX_ArticleStores_StoreId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ArticleStores",
                table: "ArticleStores",
                columns: new[] { "ArticleId", "StoreId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleStores_Articles_ArticleId",
                table: "ArticleStores",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleStores_Stores_StoreId",
                table: "ArticleStores",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleStores_Articles_ArticleId",
                table: "ArticleStores");

            migrationBuilder.DropForeignKey(
                name: "FK_ArticleStores_Stores_StoreId",
                table: "ArticleStores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ArticleStores",
                table: "ArticleStores");

            migrationBuilder.RenameTable(
                name: "ArticleStores",
                newName: "ArticleStore");

            migrationBuilder.RenameIndex(
                name: "IX_ArticleStores_StoreId",
                table: "ArticleStore",
                newName: "IX_ArticleStore_StoreId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ArticleStore",
                table: "ArticleStore",
                columns: new[] { "ArticleId", "StoreId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleStore_Articles_ArticleId",
                table: "ArticleStore",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleStore_Stores_StoreId",
                table: "ArticleStore",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
