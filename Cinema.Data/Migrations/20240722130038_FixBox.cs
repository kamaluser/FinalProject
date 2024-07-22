using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinema.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixBox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderSeat_Order_OrderId",
                table: "OrderSeat");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderSeat_Seats_SeatId",
                table: "OrderSeat");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderSeat",
                table: "OrderSeat");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "OrderSeat",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderSeat",
                table: "OrderSeat",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderSeat_OrderId",
                table: "OrderSeat",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderSeat_Order_OrderId",
                table: "OrderSeat",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderSeat_Seats_SeatId",
                table: "OrderSeat",
                column: "SeatId",
                principalTable: "Seats",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderSeat_Order_OrderId",
                table: "OrderSeat");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderSeat_Seats_SeatId",
                table: "OrderSeat");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderSeat",
                table: "OrderSeat");

            migrationBuilder.DropIndex(
                name: "IX_OrderSeat_OrderId",
                table: "OrderSeat");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "OrderSeat",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderSeat",
                table: "OrderSeat",
                columns: new[] { "OrderId", "SeatId" });

            migrationBuilder.AddForeignKey(
                name: "FK_OrderSeat_Order_OrderId",
                table: "OrderSeat",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderSeat_Seats_SeatId",
                table: "OrderSeat",
                column: "SeatId",
                principalTable: "Seats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
