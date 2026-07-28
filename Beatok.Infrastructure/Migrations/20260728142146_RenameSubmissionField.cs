using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatok.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameSubmissionField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Participation_ParticipantId",
                table: "Submissions");

            migrationBuilder.RenameColumn(
                name: "ParticipantId",
                table: "Submissions",
                newName: "ParticipationId");

            migrationBuilder.RenameIndex(
                name: "IX_Submissions_ParticipantId",
                table: "Submissions",
                newName: "IX_Submissions_ParticipationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Participation_ParticipationId",
                table: "Submissions",
                column: "ParticipationId",
                principalTable: "Participation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Participation_ParticipationId",
                table: "Submissions");

            migrationBuilder.RenameColumn(
                name: "ParticipationId",
                table: "Submissions",
                newName: "ParticipantId");

            migrationBuilder.RenameIndex(
                name: "IX_Submissions_ParticipationId",
                table: "Submissions",
                newName: "IX_Submissions_ParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Participation_ParticipantId",
                table: "Submissions",
                column: "ParticipantId",
                principalTable: "Participation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
