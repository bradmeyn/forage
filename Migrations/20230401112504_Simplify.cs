using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Forage.Migrations
{
    /// <inheritdoc />
    public partial class Simplify : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressType",
                table: "Address");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AddressType",
                table: "Address",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
