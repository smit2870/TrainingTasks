using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace taskmanagement.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewedDateToReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReviedwDate",
                table: "Review",
                newName: "ReviewedDate");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 15, 7, 10, 58, 638, DateTimeKind.Utc).AddTicks(6710), "$2a$11$DJ3eaG3NDVoeAfTmLhRym.AleC8bu9KaE/ksqXXLbysPJ4qJH7V6q" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReviewedDate",
                table: "Review",
                newName: "ReviedwDate");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 15, 6, 20, 48, 99, DateTimeKind.Utc).AddTicks(5541), "$2a$11$90E3XSOnFEgXxtFiz76Mcuw8AW39UuQhbI2sn9hvRCt8J9gvySCSi" });
        }
    }
}
