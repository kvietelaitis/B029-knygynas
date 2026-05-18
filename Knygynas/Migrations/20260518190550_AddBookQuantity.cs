using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Knygynas.Migrations
{
    /// <inheritdoc />
    public partial class AddBookQuantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Available",
                table: "Books",
                newName: "Quantity");

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "ISBN",
                keyValue: "978-0-06-112008-4",
                column: "Quantity",
                value: 10);

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "ISBN",
                keyValue: "978-0-14-028329-3",
                column: "Quantity",
                value: 10);

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "ISBN",
                keyValue: "978-0-7653-7793-1",
                column: "Quantity",
                value: 10);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Books",
                newName: "Available");

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "ISBN",
                keyValue: "978-0-06-112008-4",
                column: "Available",
                value: true);

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "ISBN",
                keyValue: "978-0-14-028329-3",
                column: "Available",
                value: true);

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "ISBN",
                keyValue: "978-0-7653-7793-1",
                column: "Available",
                value: true);
        }
    }
}
