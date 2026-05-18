using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Knygynas.Migrations
{
    /// <inheritdoc />
    public partial class AddPostomatAndSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Postomats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    City = table.Column<string>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false),
                    Company = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Postomats", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Bookstores",
                columns: new[] { "Id", "Address", "City", "CreatedDate" },
                values: new object[,]
                {
                    { 4, "Tilžės g. 109", "Šiauliai", new DateTime(2024, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, "Laisvės a. 5", "Panevėžys", new DateTime(2024, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, "Pulko g. 12", "Alytus", new DateTime(2024, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, "J. Basanavičiaus a. 8", "Marijampolė", new DateTime(2024, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, "Laisvės g. 24", "Mažeikiai", new DateTime(2024, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Postomats",
                columns: new[] { "Id", "Address", "City", "Company" },
                values: new object[,]
                {
                    { 1, "Saltoniškių g. 9 (PLC Panorama)", "Vilnius", "Omniva" },
                    { 2, "Ozo g. 25 (PPC Akropolis)", "Vilnius", "DPD" },
                    { 3, "Vokiečių g. 15", "Vilnius", "LP Express" },
                    { 4, "Karaliaus Mindaugo pr. 49 (Akropolis)", "Kaunas", "Omniva" },
                    { 5, "Savanorių pr. 255 (Rimi)", "Kaunas", "LP Express" },
                    { 6, "Taikos pr. 61 (Akropolis)", "Klaipėda", "Omniva" },
                    { 7, "H. Manto g. 90", "Klaipėda", "DPD" },
                    { 8, "Aido g. 8 (Akropolis)", "Šiauliai", "LP Express" },
                    { 9, "Klaipėdos g. 143a (Babilonas)", "Panevėžys", "Omniva" },
                    { 10, "Naujoji g. 2c", "Alytus", "DPD" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Postomats");

            migrationBuilder.DeleteData(
                table: "Bookstores",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Bookstores",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Bookstores",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Bookstores",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Bookstores",
                keyColumn: "Id",
                keyValue: 8);
        }
    }
}
