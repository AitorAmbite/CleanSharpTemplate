using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Conductor.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAtCreatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Jobs",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Jobs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "JobExecutions",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "JobExecutions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Agents",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Agents",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "JobExecutions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "JobExecutions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Agents");
        }
    }
}
