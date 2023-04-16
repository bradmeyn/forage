using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Forage.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Availabilities_AvailabilityId",
                table: "Bookings");

            migrationBuilder.DropTable(
                name: "Availabilities");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_AvailabilityId",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "AvailabilityId",
                table: "Bookings",
                newName: "Status");

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "Restaurants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "FridayCloseTime",
                table: "Restaurants",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "FridayOpenTime",
                table: "Restaurants",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "MondayCloseTime",
                table: "Restaurants",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "MondayOpenTime",
                table: "Restaurants",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OpenFriday",
                table: "Restaurants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OpenMonday",
                table: "Restaurants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OpenSaturday",
                table: "Restaurants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OpenSunday",
                table: "Restaurants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OpenThursday",
                table: "Restaurants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OpenTuesday",
                table: "Restaurants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OpenWednesday",
                table: "Restaurants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "SaturdayCloseTime",
                table: "Restaurants",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "SaturdayOpenTime",
                table: "Restaurants",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "SundayCloseTime",
                table: "Restaurants",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "SundayOpenTime",
                table: "Restaurants",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ThursdayCloseTime",
                table: "Restaurants",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ThursdayOpenTime",
                table: "Restaurants",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "TuesdayCloseTime",
                table: "Restaurants",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "TuesdayOpenTime",
                table: "Restaurants",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "WednesdayCloseTime",
                table: "Restaurants",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "WednesdayOpenTime",
                table: "Restaurants",
                type: "time without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "FridayCloseTime",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "FridayOpenTime",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "MondayCloseTime",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "MondayOpenTime",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "OpenFriday",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "OpenMonday",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "OpenSaturday",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "OpenSunday",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "OpenThursday",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "OpenTuesday",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "OpenWednesday",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "SaturdayCloseTime",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "SaturdayOpenTime",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "SundayCloseTime",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "SundayOpenTime",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "ThursdayCloseTime",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "ThursdayOpenTime",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "TuesdayCloseTime",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "TuesdayOpenTime",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "WednesdayCloseTime",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "WednesdayOpenTime",
                table: "Restaurants");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Bookings",
                newName: "AvailabilityId");

            migrationBuilder.CreateTable(
                name: "Availabilities",
                columns: table => new
                {
                    AvailabilityId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RestaurantId = table.Column<int>(type: "integer", nullable: false),
                    Filled = table.Column<bool>(type: "boolean", nullable: false),
                    MaxPartySize = table.Column<int>(type: "integer", nullable: false),
                    TimeSlot = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Availabilities", x => x.AvailabilityId);
                    table.ForeignKey(
                        name: "FK_Availabilities_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_AvailabilityId",
                table: "Bookings",
                column: "AvailabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Availabilities_RestaurantId",
                table: "Availabilities",
                column: "RestaurantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Availabilities_AvailabilityId",
                table: "Bookings",
                column: "AvailabilityId",
                principalTable: "Availabilities",
                principalColumn: "AvailabilityId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
