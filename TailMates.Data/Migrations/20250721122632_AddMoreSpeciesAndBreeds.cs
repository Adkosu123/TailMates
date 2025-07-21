using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TailMates.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreSpeciesAndBreeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Breeds",
                columns: new[] { "Id", "Name", "SpeciesId" },
                values: new object[,]
                {
                    { 7, "German Shepherd", 1 },
                    { 8, "Golden Retriever", 1 },
                    { 9, "Persian", 2 },
                    { 10, "Domestic Shorthair", 2 },
                    { 11, "Parakeet", 3 },
                    { 12, "Finch", 3 },
                    { 13, "Canary", 3 },
                    { 14, "Mini Lop", 4 },
                    { 15, "Netherland Dwarf", 4 },
                    { 16, "Flemish Giant", 4 }
                });

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateListed",
                value: new DateTime(2025, 7, 21, 12, 26, 30, 494, DateTimeKind.Utc).AddTicks(9480));

            migrationBuilder.InsertData(
                table: "Species",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 5, "Fish" },
                    { 6, "Hamster" },
                    { 7, "Snake" },
                    { 8, "Turtle" }
                });

            migrationBuilder.InsertData(
                table: "Breeds",
                columns: new[] { "Id", "Name", "SpeciesId" },
                values: new object[,]
                {
                    { 17, "Goldfish", 5 },
                    { 18, "Betta Fish", 5 },
                    { 19, "Guppy", 5 },
                    { 20, "Angelfish", 5 },
                    { 21, "Syrian Hamster", 6 },
                    { 22, "Dwarf Hamster", 6 },
                    { 23, "Roborovski Dwarf Hamster", 6 },
                    { 24, "Corn Snake", 7 },
                    { 25, "Ball Python", 7 },
                    { 26, "King Snake", 7 },
                    { 27, "Red-Eared Slider", 8 },
                    { 28, "Box Turtle", 8 },
                    { 29, "Painted Turtle", 8 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Species",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Species",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Species",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Species",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateListed",
                value: new DateTime(2025, 7, 16, 13, 32, 50, 668, DateTimeKind.Utc).AddTicks(6590));
        }
    }
}
