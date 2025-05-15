using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assignment1.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedImg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImgPath",
                schema: "Identity",
                table: "Products",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "Identity",
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImgPath",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Identity",
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImgPath",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Identity",
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImgPath",
                value: null);

            migrationBuilder.UpdateData(
                schema: "Identity",
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImgPath",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImgPath",
                schema: "Identity",
                table: "Products");
        }
    }
}
