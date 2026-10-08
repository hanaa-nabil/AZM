using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AZM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserLocatio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "UserProfiles",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LocationUpdatedAt",
                table: "UserProfiles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "UserProfiles",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShareLocation",
                table: "UserProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "LocationUpdatedAt",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "ShareLocation",
                table: "UserProfiles");
        }
    }
}
