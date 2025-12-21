using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarbeariaPortifolio.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCriadoEmEUsadoNoEmailConfirmacaoToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CriadoEm",
                table: "EmailConfirmacaoTokens",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CriadoEm",
                table: "EmailConfirmacaoTokens");
        }
    }
}
