using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarbeariaPortfolio.API.Migrations
{
    /// <inheritdoc />
    public partial class DuracaoTempoServico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DuracaoServico",
                table: "Servico",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DuracaoServico",
                table: "Servico");
        }
    }
}
