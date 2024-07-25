using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinema.Data.Migrations
{
    /// <inheritdoc />
    public partial class SessionTableUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LanguageId",
                table: "Sessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_LanguageId",
                table: "Sessions",
                column: "LanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Languages_LanguageId",
                table: "Sessions",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Languages_LanguageId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_LanguageId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "LanguageId",
                table: "Sessions");
        }
    }
}
