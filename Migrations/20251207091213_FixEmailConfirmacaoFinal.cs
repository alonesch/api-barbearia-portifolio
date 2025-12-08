using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarbeariaPortfolio.API.Migrations
{
    /// <inheritdoc />
    public partial class FixEmailConfirmacaoFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailConfirmacaoToken_Usuarios_UsuarioId",
                table: "EmailConfirmacaoToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmailConfirmacaoToken",
                table: "EmailConfirmacaoToken");

            migrationBuilder.RenameTable(
                name: "EmailConfirmacaoToken",
                newName: "EmailConfirmacaoTokens");

            migrationBuilder.RenameIndex(
                name: "IX_EmailConfirmacaoToken_UsuarioId",
                table: "EmailConfirmacaoTokens",
                newName: "IX_EmailConfirmacaoTokens_UsuarioId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmailConfirmacaoTokens",
                table: "EmailConfirmacaoTokens",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailConfirmacaoTokens_Usuarios_UsuarioId",
                table: "EmailConfirmacaoTokens",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailConfirmacaoTokens_Usuarios_UsuarioId",
                table: "EmailConfirmacaoTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmailConfirmacaoTokens",
                table: "EmailConfirmacaoTokens");

            migrationBuilder.RenameTable(
                name: "EmailConfirmacaoTokens",
                newName: "EmailConfirmacaoToken");

            migrationBuilder.RenameIndex(
                name: "IX_EmailConfirmacaoTokens_UsuarioId",
                table: "EmailConfirmacaoToken",
                newName: "IX_EmailConfirmacaoToken_UsuarioId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmailConfirmacaoToken",
                table: "EmailConfirmacaoToken",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailConfirmacaoToken_Usuarios_UsuarioId",
                table: "EmailConfirmacaoToken",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
