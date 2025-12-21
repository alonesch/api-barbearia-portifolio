using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarbeariaPortifolio.API.Migrations
{
    /// <inheritdoc />
    public partial class AjusteDeDisponibilidade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DisponibilidadeId",
                table: "Agendamento",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Agendamento_DisponibilidadeId",
                table: "Agendamento",
                column: "DisponibilidadeId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamento_Disponibilidades_DisponibilidadeId",
                table: "Agendamento",
                column: "DisponibilidadeId",
                principalTable: "Disponibilidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamento_Disponibilidades_DisponibilidadeId",
                table: "Agendamento");

            migrationBuilder.DropIndex(
                name: "IX_Agendamento_DisponibilidadeId",
                table: "Agendamento");

            migrationBuilder.DropColumn(
                name: "DisponibilidadeId",
                table: "Agendamento");
        }
    }
}
