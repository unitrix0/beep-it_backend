using Microsoft.EntityFrameworkCore.Migrations;
using System.Text;

namespace BeepBackend.Migrations
{
    public partial class AfterInsertTriggerActivityLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var qryBuilder = new StringBuilder();
            qryBuilder.AppendLine("CREATE TRIGGER AfterINSERTTriger on ActivityLogEntries")
                .AppendLine("FOR INSERT")
                .AppendLine()
                .AppendLine("AS DECLARE @EnvId INT;")
                .AppendLine("SELECT @EnvId = ins.EnvironmentId FROM INSERTED as ins;")
                .AppendLine()
                .AppendLine("IF ((SELECT Count(Id) as Cnt FROM ActivityLogEntries WHERE EnvironmentId = @EnvId) > 4)")
                .AppendLine("BEGIN")
                .AppendLine(
                    "DELETE FROM ActivityLogEntries WHERE Id in (SELECT id FROM ActivityLogEntries ORDER BY ActionDate Desc OFFSET 4 ROWS)")
                .AppendLine("END");

            migrationBuilder.Sql(qryBuilder.ToString());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER[dbo].[AfterINSERTTriger]");
        }
    }
}
