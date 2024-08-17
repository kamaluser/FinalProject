using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinema.Data.Migrations
{
    /// <inheritdoc />
    public partial class SentReminderAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookedFrom",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "BookedUntil",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "IsOrdered",
                table: "Seats");

            migrationBuilder.CreateTable(
                name: "SentReminders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SentReminders", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SentReminders");

            migrationBuilder.AddColumn<DateTime>(
                name: "BookedFrom",
                table: "Seats",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BookedUntil",
                table: "Seats",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOrdered",
                table: "Seats",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
