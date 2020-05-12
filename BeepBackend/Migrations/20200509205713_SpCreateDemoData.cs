using System.Text;
using BeepBackend.Helpers;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BeepBackend.Migrations
{
    public partial class SpCreateDemoData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var qry = new StringBuilder();
            qry.AppendLine("-- =============================================")
                .AppendLine("-- Author:		M. Rebsamen")
                .AppendLine("-- Create date: 09.05.2020")
                .AppendLine("-- Description:	Erzeugt Demo Daten für die Angegebene UserId")
                .AppendLine("-- =============================================")
                .AppendLine("CREATE PROCEDURE [dbo].[CreateDemoDataForUser]")
                .AppendLine("-- Add the parameters for the stored procedure here")
                .AppendLine("--<@Param2, sysname, @p2> <Datatype_For_Param2, , int> = <Default_Value_For_Param2, , 0>")
                .AppendLine("@UserId int")
                .AppendLine("AS")
                .AppendLine("BEGIN")
                .AppendLine("DECLARE @environmentId INT;")
                .AppendLine("SET @environmentId = (SELECT Id FROM Environments WHERE UserId = @UserId)")
                .AppendLine("")
                .AppendLine("")
                .AppendLine("-- Create ArticleUserSettings")
                .AppendLine(
                    "INSERT INTO ArticleUserSettings (ArticleId, EnvironmentId, KeepStockAmount, ArticleGroupId, UsualLifetime)")
                .AppendLine("SELECT Id,")
                .AppendLine("	@environmentId,")
                .AppendLine("	(SELECT abs(convert(varbinary, NewId()) % 5) as nr),")
                .AppendLine("	1,")
                .AppendLine("	0")
                .AppendLine("FROM Articles")
                .AppendLine("	")
                .AppendLine("-- Create Stock Entries")
                .AppendLine("INSERT INTO StockEntries (ArticleId, EnvironmentId)")
                .AppendLine("SELECT Id,")
                .AppendLine("	@environmentId")
                .AppendLine("FROM Articles")
                .AppendLine("")
                .AppendLine("-- StockEntryValues")
                .AppendLine(
                    "INSERT INTO StockEntryValues (ArticleId, EnvironmentId, IsOpened, AmountOnStock, ExpireDate, OpenedOn, AmountRemaining)")
                .AppendLine("SELECT Id as ArticleId,")
                .AppendLine("	@environmentId as EnvironmentId,")
                .AppendLine("	1,")
                .AppendLine("	1 as AmountOnStock,")
                .AppendLine(
                    "	(SELECT DATEADD(m,(SELECT abs(convert(varbinary, NewId()) % 12)), GETDATE())) as ExpireDate,")
                .AppendLine("	'0001-01-01 00:00:00.0000000' as OpenedOn,")
                .AppendLine("	1 as AmountRemaining")
                .AppendLine("FROM Articles")
                .AppendLine("	")
                .AppendLine(
                    "INSERT INTO StockEntryValues (ArticleId, EnvironmentId, IsOpened, AmountOnStock, ExpireDate, OpenedOn, AmountRemaining)")
                .AppendLine("SELECT Id as ArticleId,")
                .AppendLine("	@environmentId as EnvironmentId,")
                .AppendLine("	0,")
                .AppendLine("	(SELECT abs(convert(varbinary, NewId()) %5)+1 as nr) as AmountOnStock,")
                .AppendLine(
                    "	(SELECT DATEADD(m,(SELECT abs(convert(varbinary, NewId()) % 12)), GETDATE())) as ExpireDate,")
                .AppendLine("	'0001-01-01 00:00:00.0000000' as OpenedOn,")
                .AppendLine("	1 as AmountRemaining")
                .AppendLine("FROM Articles")
                .AppendLine("")
                .AppendLine("")
                .AppendLine("UPDATE StockEntryValues ")
                .AppendLine("Set AmountRemaining = ROUND(1.0/(SELECT abs(convert(varbinary, NewId()) %3)+2 as nr),2)")
                .AppendLine("WHERE IsOpened = 1 AND EnvironmentId = @environmentId")
                .AppendLine("END")
                .AppendLine("GO");

            migrationBuilder.Sql(qry.ToString());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE [dbo].[CreateDemoDataForUser]");
        }
    }
}
