using System.Text;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class UpdateShoppingListView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var qry = new StringBuilder();
            qry.AppendLine("IF EXISTS(select * FROM sys.views where name = 'ShoppingList') BEGIN")
                .AppendLine("DROP VIEW [dbo].[ShoppingList]")
                .AppendLine("END")
                .AppendLine("GO")
                .AppendLine("CREATE VIEW [dbo].[ShoppingList]")
                .AppendLine("AS")
                .AppendLine(
                    "SELECT Name AS StoreName, Barcode, ArticleName, ImageUrl, UnitAbbreviation, EnvironmentId, KeepStockAmount, OnStock, Opened, KeepStockAmount - OnStock AS Needed, ContentAmount * ISNULL(AmountRemaining, 0) ")
                .AppendLine(" AS AmountRemaining")
                .AppendLine(
                    "FROM (SELECT s.Name, a.Barcode, a.Name AS ArticleName, a.ImageUrl, a.ContentAmount, au.Abbreviation AS UnitAbbreviation, aus.EnvironmentId, aus.KeepStockAmount,")
                .AppendLine(" (SELECT ISNULL(SUM(AmountOnStock), 0) AS Expr1")
                .AppendLine(" FROM dbo.StockEntryValues AS sev")
                .AppendLine(
                    " WHERE (EnvironmentId = aus.EnvironmentId) AND (IsOpened = 0) AND (ArticleId = a.Id)) AS OnStock,")
                .AppendLine(" (SELECT ISNULL(SUM(AmountOnStock), 0) AS Expr1")
                .AppendLine(" FROM dbo.StockEntryValues AS sev")
                .AppendLine(
                    " WHERE (EnvironmentId = aus.EnvironmentId) AND (IsOpened = 1) AND (ArticleId = a.Id)) AS Opened,")
                .AppendLine(" (SELECT SUM(AmountRemaining) AS Expr1")
                .AppendLine(" FROM dbo.StockEntryValues AS sev")
                .AppendLine(
                    " WHERE (EnvironmentId = aus.EnvironmentId) AND (IsOpened = 1) AND (ArticleId = a.Id)) AS AmountRemaining")
                .AppendLine(" FROM dbo.Stores AS s INNER JOIN")
                .AppendLine(" dbo.ArticleStores AS ast ON s.Id = ast.StoreId INNER JOIN")
                .AppendLine(" dbo.Articles AS a ON ast.ArticleId = a.Id INNER JOIN")
                .AppendLine(" dbo.ArticleUserSettings AS aus ON a.Id = aus.ArticleId LEFT OUTER JOIN")
                .AppendLine(" dbo.ArticleUnits AS au ON a.UnitId = au.Id")
                .AppendLine(" WHERE (aus.ArticleGroupId = 1)) AS sub")
                .AppendLine("GO");


            migrationBuilder.Sql(qry.ToString());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW [dbo].[ShoppingList]");
        }
    }
}
