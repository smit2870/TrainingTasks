using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace taskmanagement.Migrations
{
    /// <inheritdoc />
    public partial class CreateTaskSubmission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Submission",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TaskAssignmentId = table.Column<int>(type: "int", nullable: false),
                    SubmissionUrl = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Notes = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SubmittedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Submission_TaskAssignment_TaskAssignmentId",
                        column: x => x.TaskAssignmentId,
                        principalTable: "TaskAssignment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 12, 9, 47, 52, 192, DateTimeKind.Utc).AddTicks(2916), "$2a$11$NjRsBzlpmAD.vgnxEnXzl.CsJpJdInjXnRh870Ba0124i6i5Eqi9S" });

            migrationBuilder.CreateIndex(
                name: "IX_Submission_TaskAssignmentId",
                table: "Submission",
                column: "TaskAssignmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Submission");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 12, 7, 14, 56, 164, DateTimeKind.Utc).AddTicks(9003), "$2a$11$0Q6FnGK5GDYJkPUVhVSEmu32i.fBqeU5KUqnmWAAtWrSNTxIgi462" });
        }
    }
}
