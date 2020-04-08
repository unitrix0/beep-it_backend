using System.Text;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class AddShoppingListGroupsView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var qry = new StringBuilder();
            qry.AppendLine("CREATE VIEW [dbo].[ShoppingListGroups]")
                .AppendLine("AS")
                .AppendLine(
                    "SELECT DISTINCT Id, GroupName, EnvironmentId, KeepStockAmount, OnStock, KeepStockAmount - OnStock AS Needed")
                .AppendLine("FROM (SELECT ag.Id, ag.Name AS GroupName, aus2.EnvironmentId, ag.KeepStockAmount,")
                .AppendLine(" (SELECT ISNULL(SUM(sev.AmountOnStock), 0) AS Expr1")
                .AppendLine(" FROM dbo.ArticleUserSettings AS aus INNER JOIN")
                .AppendLine(
                    " dbo.StockEntryValues AS sev ON aus.ArticleId = sev.ArticleId AND aus.EnvironmentId = sev.EnvironmentId")
                .AppendLine(
                    " WHERE (aus.ArticleGroupId = ag.Id) AND (sev.IsOpened = 0) AND (aus.EnvironmentId = aus2.EnvironmentId)) AS OnStock")
                .AppendLine(" FROM dbo.ArticleGroups AS ag INNER JOIN")
                .AppendLine(" dbo.ArticleUserSettings AS aus2 ON ag.Id = aus2.ArticleGroupId")
                .AppendLine(" WHERE (ag.Id > 1)) AS sub")
                .AppendLine("GO");

            migrationBuilder.Sql(qry.ToString());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW [dbo].[ShoppingListGroups]");
        }
    }
}
