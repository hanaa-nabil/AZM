using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AZM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSportTypeToEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventRouteWaypoints");

            migrationBuilder.DropTable(
                name: "LiveLocations");

            migrationBuilder.DropTable(
                name: "EventRoutes");

            migrationBuilder.DropTable(
                name: "LiveSessions");



            migrationBuilder.AddColumn<int>(
                name: "SportType",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeetingAddress",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "MeetingLat",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "MeetingLng",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "SportType",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "FcmToken",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "EventRoutes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DistanceMeters = table.Column<double>(type: "float", nullable: true),
                    EndAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndLatitude = table.Column<double>(type: "float", nullable: false),
                    EndLongitude = table.Column<double>(type: "float", nullable: false),
                    EstimatedDurationSeconds = table.Column<int>(type: "int", nullable: true),
                    StartAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartLatitude = table.Column<double>(type: "float", nullable: false),
                    StartLongitude = table.Column<double>(type: "float", nullable: false)
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
                name: "LiveSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EndedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LiveSessions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LiveSessions_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

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

            migrationBuilder.CreateTable(
                name: "LiveLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LiveSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccuracyMeters = table.Column<double>(type: "float", nullable: true),
                    HeadingDegrees = table.Column<double>(type: "float", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    RecordedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SpeedMetersPerSecond = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LiveLocations_LiveSessions_LiveSessionId",
                        column: x => x.LiveSessionId,
                        principalTable: "LiveSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventRoutes_EventId",
                table: "EventRoutes",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventRouteWaypoints_EventRouteId",
                table: "EventRouteWaypoints",
                column: "EventRouteId");

            migrationBuilder.CreateIndex(
                name: "IX_LiveLocations_LiveSessionId",
                table: "LiveLocations",
                column: "LiveSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_LiveSessions_EventId",
                table: "LiveSessions",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_LiveSessions_UserId",
                table: "LiveSessions",
                column: "UserId");
        }
    }
}
