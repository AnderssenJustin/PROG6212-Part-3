using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PROG6212_PART_3.Migrations
{
    /// <inheritdoc />
    public partial class updatedforloginfeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HourlyRate",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "LecturerName",
                table: "Claims");

            migrationBuilder.RenameColumn(
                name: "ApprovedDate",
                table: "Claims",
                newName: "ManagerApprovedDate");

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByCoordinator",
                table: "Claims",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByManager",
                table: "Claims",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CoordinatorApprovedDate",
                table: "Claims",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Claims",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Role = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    HourlyRate = table.Column<double>(type: "REAL", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CreatedDate", "Email", "FirstName", "HourlyRate", "IsActive", "LastLogin", "LastName", "PasswordHash", "Role", "Username" },
                values: new object[] { 1, new DateTime(2025, 11, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "hr@university.edu", "HR", 0.0, true, null, "Administrator", "HR@2025", "HR", "hradmin" });

            migrationBuilder.CreateIndex(
                name: "IX_Claims_UserId",
                table: "Claims",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_Users_UserId",
                table: "Claims",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Claims_Users_UserId",
                table: "Claims");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Claims_UserId",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "ApprovedByCoordinator",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "ApprovedByManager",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "CoordinatorApprovedDate",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Claims");

            migrationBuilder.RenameColumn(
                name: "ManagerApprovedDate",
                table: "Claims",
                newName: "ApprovedDate");

            migrationBuilder.AddColumn<double>(
                name: "HourlyRate",
                table: "Claims",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "LecturerName",
                table: "Claims",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
