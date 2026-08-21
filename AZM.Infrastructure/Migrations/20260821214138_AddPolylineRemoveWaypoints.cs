using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AZM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPolylineRemoveWaypoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventRouteWaypoints");

            migrationBuilder.AddColumn<string>(
                name: "Polyline",
                table: "EventRoutes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Polyline",
                table: "EventRoutes");

            migrationBuilder.CreateTable(
                name: "EventRouteWaypoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventRouteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventRouteWaypoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventRouteWaypoints_EventRoutes_EventRouteId",
                        column: x => x.EventRouteId,
                        principalTable: "EventRoutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventRouteWaypoints_EventRouteId",
                table: "EventRouteWaypoints",
                column: "EventRouteId");
        }
    }
}
