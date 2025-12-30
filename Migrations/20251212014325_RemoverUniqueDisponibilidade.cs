using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarbeariaPortifolio.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoverUniqueDisponibilidade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP INDEX IF EXISTS ""IX_Agendamento_DisponibilidadeId"";

                CREATE INDEX ""IX_Agendamento_DisponibilidadeId""
                ON ""Agendamento"" (""DisponibilidadeId"");
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP INDEX IF EXISTS ""IX_Agendamento_DisponibilidadeId"";

                CREATE UNIQUE INDEX ""IX_Agendamento_DisponibilidadeId""
                ON ""Agendamento"" (""DisponibilidadeId"");
            ");
        }
    }
}
