using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarbeariaPortfolio.API.Migrations
{
    /// <inheritdoc />
    public partial class AjusteAgendamentoObservacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Barbeiro_Usuarios_UsuarioId",
                table: "Barbeiro");

            migrationBuilder.RenameColumn(
                name: "Senha",
                table: "Usuarios",
                newName: "SenhaHash");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Servico",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Revoked",
                table: "RefreshTokens",
                newName: "Revogado");

            migrationBuilder.RenameColumn(
                name: "ExpiresAtUtc",
                table: "RefreshTokens",
                newName: "ExpiraEm");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "RefreshTokens",
                newName: "CriadoEm");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Cliente",
                newName: "Id");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "NomeCompleto",
                keyValue: null,
                column: "NomeCompleto",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "NomeCompleto",
                table: "Usuarios",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Cargo",
                keyValue: null,
                column: "Cargo",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Cargo",
                table: "Usuarios",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Usuarios",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TokenHash",
                table: "RefreshTokens",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(300)",
                oldMaxLength: 300)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataCadastro",
                table: "Cliente",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2025, 11, 13, 3, 25, 49, 375, DateTimeKind.Utc).AddTicks(3105),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2025, 11, 10, 0, 51, 12, 408, DateTimeKind.Utc).AddTicks(7381));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataRegistro",
                table: "Agendamento",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2025, 11, 13, 3, 25, 49, 375, DateTimeKind.Utc).AddTicks(3705),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 11, 10, 0, 51, 12, 408, DateTimeKind.Utc).AddTicks(7941));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataHora",
                table: "Agendamento",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AddColumn<string>(
                name: "Observacao",
                table: "Agendamento",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_Barbeiro_Usuarios_UsuarioId",
                table: "Barbeiro",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Barbeiro_Usuarios_UsuarioId",
                table: "Barbeiro");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Observacao",
                table: "Agendamento");

            migrationBuilder.RenameColumn(
                name: "SenhaHash",
                table: "Usuarios",
                newName: "Senha");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Servico",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Revogado",
                table: "RefreshTokens",
                newName: "Revoked");

            migrationBuilder.RenameColumn(
                name: "ExpiraEm",
                table: "RefreshTokens",
                newName: "ExpiresAtUtc");

            migrationBuilder.RenameColumn(
                name: "CriadoEm",
                table: "RefreshTokens",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Cliente",
                newName: "ID");

            migrationBuilder.AlterColumn<string>(
                name: "NomeCompleto",
                table: "Usuarios",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Cargo",
                table: "Usuarios",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TokenHash",
                table: "RefreshTokens",
                type: "varchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataCadastro",
                table: "Cliente",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2025, 11, 10, 0, 51, 12, 408, DateTimeKind.Utc).AddTicks(7381),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2025, 11, 13, 3, 25, 49, 375, DateTimeKind.Utc).AddTicks(3105));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataRegistro",
                table: "Agendamento",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 11, 10, 0, 51, 12, 408, DateTimeKind.Utc).AddTicks(7941),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2025, 11, 13, 3, 25, 49, 375, DateTimeKind.Utc).AddTicks(3705));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataHora",
                table: "Agendamento",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AddForeignKey(
                name: "FK_Barbeiro_Usuarios_UsuarioId",
                table: "Barbeiro",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
