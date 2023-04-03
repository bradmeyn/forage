using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Forage.Migrations
{
    /// <inheritdoc />
    public partial class Updateagain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Address");

            migrationBuilder.AddColumn<int>(
                name: "PostCode",
                table: "Address",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostCode",
                table: "Address");

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Address",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
