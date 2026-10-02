using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AZM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removevisibility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>("IsPrivate", "Events", nullable: false, defaultValue: false);
            migrationBuilder.AddColumn<bool>("IsPink", "Events", nullable: false, defaultValue: false);

            migrationBuilder.Sql(@"UPDATE Events SET IsPrivate = 1 WHERE Visibility = 1;
                       UPDATE Events SET IsPink = 1 WHERE Visibility = 2;");

            migrationBuilder.DropColumn("Visibility", "Events");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPink",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "Events");

            migrationBuilder.AddColumn<int>(
                name: "Visibility",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
