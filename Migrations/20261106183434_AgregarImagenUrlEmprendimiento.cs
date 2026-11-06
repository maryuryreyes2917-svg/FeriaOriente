using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeriaOriente.API.Migrations
{
    /// <inheritdoc />
    public partial class AgregarImagenUrlEmprendimiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "Emprendimientos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "Emprendimientos");
        }
    }
}
