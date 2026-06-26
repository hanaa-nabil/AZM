using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AZM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSportsAndPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePhotoUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EventRoutes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartLatitude = table.Column<double>(type: "float", nullable: false),
                    StartLongitude = table.Column<double>(type: "float", nullable: false),
                    StartAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndLatitude = table.Column<double>(type: "float", nullable: false),
                    EndLongitude = table.Column<double>(type: "float", nullable: false),
                    EndAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DistanceMeters = table.Column<double>(type: "float", nullable: true),
                    EstimatedDurationSeconds = table.Column<int>(type: "int", nullable: true),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventRoutes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventRoutes_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Sport = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSports_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventRouteWaypoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    EventRouteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                name: "IX_EventRoutes_EventId",
                table: "EventRoutes",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventRouteWaypoints_EventRouteId",
                table: "EventRouteWaypoints",
                column: "EventRouteId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSports_UserId_Sport",
                table: "UserSports",
                columns: new[] { "UserId", "Sport" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventRouteWaypoints");

            migrationBuilder.DropTable(
                name: "UserSports");

            migrationBuilder.DropTable(
                name: "EventRoutes");

            migrationBuilder.DropColumn(
                name: "ProfilePhotoUrl",
                table: "AspNetUsers");
        }
    }
}
