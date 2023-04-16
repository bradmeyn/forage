using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Forage.Migrations
{
    /// <inheritdoc />
    public partial class OpeningHoursChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.RenameColumn(
                name: "WednesdayOpenTime",
                table: "Restaurants",
                newName: "WeekendOpen");

            migrationBuilder.RenameColumn(
                name: "WednesdayCloseTime",
                table: "Restaurants",
                newName: "WeekendClose");

            migrationBuilder.RenameColumn(
                name: "TuesdayOpenTime",
                table: "Restaurants",
                newName: "WeekdayOpen");

            migrationBuilder.RenameColumn(
                name: "TuesdayCloseTime",
                table: "Restaurants",
                newName: "WeekdayClose");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WeekendOpen",
                table: "Restaurants",
                newName: "WednesdayOpenTime");

            migrationBuilder.RenameColumn(
                name: "WeekendClose",
                table: "Restaurants",
                newName: "WednesdayCloseTime");

            migrationBuilder.RenameColumn(
                name: "WeekdayOpen",
                table: "Restaurants",
                newName: "TuesdayOpenTime");

            migrationBuilder.RenameColumn(
                name: "WeekdayClose",
                table: "Restaurants",
                newName: "TuesdayCloseTime");

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
        }
    }
}
