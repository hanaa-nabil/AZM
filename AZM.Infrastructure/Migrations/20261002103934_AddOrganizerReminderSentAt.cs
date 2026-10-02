using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AZM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizerReminderSentAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OrganizerReminderSentAt",
                table: "Events",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrganizerReminderSentAt",
                table: "Events");
        }
    }
}
