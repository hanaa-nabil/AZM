using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AZM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVisbility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Events");

            migrationBuilder.AddColumn<int>(
                name: "Visibility",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Visibility",
                table: "Events");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Events",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }
    }
}
