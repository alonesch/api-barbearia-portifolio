using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarbeariaPortfolio.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRoleFromUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Disponibilidades",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Disponibilidades",
                newName: "id");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Usuarios",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
