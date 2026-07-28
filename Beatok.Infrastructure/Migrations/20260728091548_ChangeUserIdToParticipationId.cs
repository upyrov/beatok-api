using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatok.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUserIdToParticipationId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scores_Users_UserId",
                table: "Scores");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Scores",
                newName: "ParticipationId");

            migrationBuilder.RenameIndex(
                name: "IX_Scores_UserId",
                table: "Scores",
                newName: "IX_Scores_ParticipationId");

            migrationBuilder.RenameIndex(
                name: "IX_Scores_SubmissionId_UserId",
                table: "Scores",
                newName: "IX_Scores_SubmissionId_ParticipationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Scores_Participation_ParticipationId",
                table: "Scores",
                column: "ParticipationId",
                principalTable: "Participation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scores_Participation_ParticipationId",
                table: "Scores");

            migrationBuilder.RenameColumn(
                name: "ParticipationId",
                table: "Scores",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Scores_SubmissionId_ParticipationId",
                table: "Scores",
                newName: "IX_Scores_SubmissionId_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Scores_ParticipationId",
                table: "Scores",
                newName: "IX_Scores_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Scores_Users_UserId",
                table: "Scores",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
