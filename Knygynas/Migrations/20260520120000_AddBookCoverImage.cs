using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Knygynas.Migrations
{
    /// <inheritdoc />
    public partial class AddBookCoverImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverImageUrl",
                table: "Books",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "ISBN",
                keyValue: "978-0-14-028329-3",
                column: "CoverImageUrl",
                value: "/images/books/1984.svg");

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "ISBN",
                keyValue: "978-0-06-112008-4",
                column: "CoverImageUrl",
                value: "/images/books/to-kill-a-mockingbird.svg");

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "ISBN",
                keyValue: "978-0-7653-7793-1",
                column: "CoverImageUrl",
                value: "/images/books/dune.svg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverImageUrl",
                table: "Books");
        }
    }
}
