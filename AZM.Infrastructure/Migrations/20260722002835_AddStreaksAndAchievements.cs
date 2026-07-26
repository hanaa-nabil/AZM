using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AZM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStreaksAndAchievements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Notifications_RecipientId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Achievements_UserId",
                table: "Achievements");

            migrationBuilder.AddColumn<int>(
                name: "CurrentStreak",
                table: "UserProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateOnly>(
                name: "LastActiveDate",
                table: "UserProfiles",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LongestStreak",
                table: "UserProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StreakFreezesAvailable",
                table: "UserProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "AchievementDefinitionId",
                table: "Achievements",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "AchievementDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IconUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CriteriaType = table.Column<int>(type: "int", nullable: false),
                    Threshold = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchievementDefinitions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RecipientId_Type_CreatedAt",
                table: "Notifications",
                columns: new[] { "RecipientId", "Type", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_AchievementDefinitionId",
                table: "Achievements",
                column: "AchievementDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_UserId_AchievementDefinitionId",
                table: "Achievements",
                columns: new[] { "UserId", "AchievementDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AchievementDefinitions_Code",
                table: "AchievementDefinitions",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Achievements_AchievementDefinitions_AchievementDefinitionId",
                table: "Achievements",
                column: "AchievementDefinitionId",
                principalTable: "AchievementDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Achievements_AchievementDefinitions_AchievementDefinitionId",
                table: "Achievements");

            migrationBuilder.DropTable(
                name: "AchievementDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_RecipientId_Type_CreatedAt",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Achievements_AchievementDefinitionId",
                table: "Achievements");

            migrationBuilder.DropIndex(
                name: "IX_Achievements_UserId_AchievementDefinitionId",
                table: "Achievements");

            migrationBuilder.DropColumn(
                name: "CurrentStreak",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "LastActiveDate",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "LongestStreak",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "StreakFreezesAvailable",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "AchievementDefinitionId",
                table: "Achievements");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RecipientId",
                table: "Notifications",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_UserId",
                table: "Achievements",
                column: "UserId");
        }
    }
}
