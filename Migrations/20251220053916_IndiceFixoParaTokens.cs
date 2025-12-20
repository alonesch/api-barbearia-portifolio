using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarbeariaPortfolio.API.Migrations
{
    /// <inheritdoc />
    public partial class IndiceFixoParaTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EmailConfirmacaoTokens_Token",
                table: "EmailConfirmacaoTokens",
                column: "Token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmailConfirmacaoTokens_Token",
                table: "EmailConfirmacaoTokens");
        }
    }
}
