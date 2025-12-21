using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarbeariaPortifolio.API.Migrations
{
    /// <inheritdoc />
    public partial class SlotHora : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HoraFim",
                table: "Disponibilidades");

            migrationBuilder.DropColumn(
                name: "HoraInicio",
                table: "Disponibilidades");

            migrationBuilder.AddColumn<string>(
                name: "Hora",
                table: "Disponibilidades",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hora",
                table: "Disponibilidades");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "HoraFim",
                table: "Disponibilidades",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "HoraInicio",
                table: "Disponibilidades",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }
    }
}
