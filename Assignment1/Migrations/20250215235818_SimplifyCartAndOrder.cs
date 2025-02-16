using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Assignment1.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyCartAndOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderLists_OrderListId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "OrderLists");

            migrationBuilder.DropIndex(
                name: "IX_Orders_OrderListId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderListId",
                table: "Orders");

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "OrderId", "OrderDate" },
                values: new object[] { 1, new DateTime(2023, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "OrderId",
                keyValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "OrderListId",
                table: "Orders",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrderLists",
                columns: table => new
                {
                    OrderListId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderLists", x => x.OrderListId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderListId",
                table: "Orders",
                column: "OrderListId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderLists_OrderListId",
                table: "Orders",
                column: "OrderListId",
                principalTable: "OrderLists",
                principalColumn: "OrderListId");
        }
    }
}
