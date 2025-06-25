using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TailMates.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialSeedDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "f1a36e3c-bfa8-4a53-ab16-916d395ca40b", 0, "aa843b2a-8042-4105-9f80-fb6a77d17b4f", "ApplicationUser", "admin@tailmates.com", true, "Admin", "Administrator", false, null, "ADMIN@TAILMATES.COM", "ADMIN@TAILMATES.COM", "AQAAAAIAAYagAAAAEFzenO3D+d3O+dEpF1/rsliCuAB4TE09UQD03Fj9YWk/Oslgn0/EM9wkCdu0hCh6AQ==", null, false, "2fcbe166-e1cf-4cab-b6a3-81557b39daf9", false, "admin@tailmates.com" });

            migrationBuilder.InsertData(
                table: "Shelters",
                columns: new[] { "Id", "Address", "Email", "Name", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, "123 Pet Lane, Animal City", "info@happypaws.com", "Happy Paws Shelter", "555-111-2222" },
                    { 2, "456 Sunny Blvd, Green Valley", "contact@sunshinesanctuary.org", "Sunshine Sanctuary", "555-333-4444" }
                });

            migrationBuilder.InsertData(
                table: "Species",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Dog" },
                    { 2, "Cat" },
                    { 3, "Bird" },
                    { 4, "Rabbit" }
                });

            migrationBuilder.InsertData(
                table: "Breeds",
                columns: new[] { "Id", "Name", "SpeciesId" },
                values: new object[,]
                {
                    { 1, "Labrador Retriever", 1 },
                    { 2, "Siamese", 2 },
                    { 3, "Poodle", 1 },
                    { 4, "Maine Coon", 2 },
                    { 5, "Parrot", 3 },
                    { 6, "Dutch", 4 }
                });

            migrationBuilder.InsertData(
                table: "Pets",
                columns: new[] { "Id", "Age", "BreedId", "DateListed", "Description", "Gender", "ImageUrl", "IsAdopted", "Name", "ShelterId", "SpeciesId" },
                values: new object[,]
                {
                    { 1, 3, 1, new DateTime(2025, 6, 25, 7, 3, 46, 871, DateTimeKind.Utc).AddTicks(1198), "A friendly and energetic dog looking for a loving home.", "Male", "https://th.bing.com/th/id/OIP.3J2q-ML2eSU3xPhgV4ez0AHaE8?r=0&rs=1&pid=ImgDetMain", false, "Rexy", 1, 1 },
                    { 2, 2, 2, new DateTime(2024, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "A curious and playful cat who loves cuddles.", "Female", "https://th.bing.com/th/id/OIP.cvg_MdgYsY9-fKD5eV8SpgHaE5?r=0&rs=1&pid=ImgDetMain", false, "Whiskers", 1, 2 },
                    { 3, 1, 3, new DateTime(2024, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "A small, fluffy dog with a big personality, great with kids.", "Male", "https://www.thesprucepets.com/thmb/93mObWq1NDdH6dZkibYq0XO4ZaU=/2121x1414/filters:no_upscale():max_bytes(150000):strip_icc()/Pomeranianstandingonroad-7defae876b0f44589279e188c95666ea.jpg", false, "Buddy", 2, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f1a36e3c-bfa8-4a53-ab16-916d395ca40b");

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Shelters",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Shelters",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Species",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Species",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Species",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Species",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
