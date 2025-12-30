using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarbeariaPortifolio.API.Migrations
{
    /// <inheritdoc />
    public partial class AjusteClientePerfil : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamento_Cliente_ClienteId",
                table: "Agendamento");

            migrationBuilder.DropColumn(
                name: "Nome",
                table: "Cliente");

            migrationBuilder.RenameColumn(
                name: "ClienteId",
                table: "Agendamento",
                newName: "UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_Agendamento_ClienteId",
                table: "Agendamento",
                newName: "IX_Agendamento_UsuarioId");

            migrationBuilder.AlterColumn<string>(
                name: "Cpf",
                table: "Cliente",
                type: "varchar(15)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(15)");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Cliente",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_UsuarioId",
                table: "Cliente",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamento_Usuarios_UsuarioId",
                table: "Agendamento",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cliente_Usuarios_UsuarioId",
                table: "Cliente",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamento_Usuarios_UsuarioId",
                table: "Agendamento");

            migrationBuilder.DropForeignKey(
                name: "FK_Cliente_Usuarios_UsuarioId",
                table: "Cliente");

            migrationBuilder.DropIndex(
                name: "IX_Cliente_UsuarioId",
                table: "Cliente");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Cliente");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "Agendamento",
                newName: "ClienteId");

            migrationBuilder.RenameIndex(
                name: "IX_Agendamento_UsuarioId",
                table: "Agendamento",
                newName: "IX_Agendamento_ClienteId");

            migrationBuilder.AlterColumn<string>(
                name: "Cpf",
                table: "Cliente",
                type: "varchar(15)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(15)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "Cliente",
                type: "varchar(150)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamento_Cliente_ClienteId",
                table: "Agendamento",
                column: "ClienteId",
                principalTable: "Cliente",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
