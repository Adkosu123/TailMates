using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TailMates.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesAndSeedAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f1a36e3c-bfa8-4a53-ab16-916d395ca40b");

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateListed",
                value: new DateTime(2025, 7, 14, 8, 38, 5, 957, DateTimeKind.Utc).AddTicks(4614));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "f1a36e3c-bfa8-4a53-ab16-916d395ca40b", 0, "e44dd85b-381b-49cf-a088-5046a4827517", "ApplicationUser", "admin@tailmates.com", true, "Admin", "Administrator", false, null, "ADMIN@TAILMATES.COM", "ADMIN@TAILMATES.COM", "AQAAAAIAAYagAAAAEIhoDTh6t1GrTO04Al57licw4uBdzJ4GQk5bpBnaRiYX9RF/oxALAwXWfELLBBV+7Q==", null, false, "114075a4-28f8-44fa-b6c6-e4ad1b65a1eb", false, "admin@tailmates.com" });

            migrationBuilder.UpdateData(
                table: "Pets",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateListed",
                value: new DateTime(2025, 7, 14, 8, 16, 57, 269, DateTimeKind.Utc).AddTicks(3313));
        }
    }
}
