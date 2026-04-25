using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CueConnect757.DataSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddApaSyncTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastSyncedAt",
                table: "Teams",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SyncSource",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSyncedAt",
                table: "Sessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SyncSource",
                table: "Sessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSyncedAt",
                table: "Players",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SyncSource",
                table: "Players",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSyncedAt",
                table: "Matches",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SyncSource",
                table: "Matches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSyncedAt",
                table: "Divisions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SyncSource",
                table: "Divisions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastSyncedAt",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "SyncSource",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "LastSyncedAt",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "SyncSource",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "LastSyncedAt",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "SyncSource",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "LastSyncedAt",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "SyncSource",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "LastSyncedAt",
                table: "Divisions");

            migrationBuilder.DropColumn(
                name: "SyncSource",
                table: "Divisions");
        }
    }
}
