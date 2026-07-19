using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatok.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAtToScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scores_Submissions_SubmissionId1",
                table: "Scores");

            migrationBuilder.DropIndex(
                name: "IX_Scores_SubmissionId1",
                table: "Scores");

            migrationBuilder.DropColumn(
                name: "SubmissionId1",
                table: "Scores");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Scores",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Scores");

            migrationBuilder.AddColumn<Guid>(
                name: "SubmissionId1",
                table: "Scores",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Scores_SubmissionId1",
                table: "Scores",
                column: "SubmissionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Scores_Submissions_SubmissionId1",
                table: "Scores",
                column: "SubmissionId1",
                principalTable: "Submissions",
                principalColumn: "Id");
        }
    }
}
