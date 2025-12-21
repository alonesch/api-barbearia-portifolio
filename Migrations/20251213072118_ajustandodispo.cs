using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarbeariaPortifolio.API.Migrations
{
    /// <inheritdoc />
    public partial class ajustandodispo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DisponibilidadeId",
                table: "Agendamento",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DisponibilidadeId",
                table: "Agendamento",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
