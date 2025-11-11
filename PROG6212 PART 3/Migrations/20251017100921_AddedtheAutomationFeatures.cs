using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PROG6212_PART_3.Migrations
{
    /// <inheritdoc />
    public partial class AddedtheAutomationFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "Claims",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAmountValid",
                table: "Claims",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDocumentValid",
                table: "Claims",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHoursValid",
                table: "Claims",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Claims",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "IsAmountValid",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "IsDocumentValid",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "IsHoursValid",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Claims");
        }
    }
}
