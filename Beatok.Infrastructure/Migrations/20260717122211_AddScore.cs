using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatok.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Score_Lobbies_LobbyId",
                table: "Score");

            migrationBuilder.DropForeignKey(
                name: "FK_Score_Submissions_SubmissionId",
                table: "Score");

            migrationBuilder.DropForeignKey(
                name: "FK_Score_Submissions_SubmissionId1",
                table: "Score");

            migrationBuilder.DropForeignKey(
                name: "FK_Score_Users_UserId",
                table: "Score");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Score",
                table: "Score");

            migrationBuilder.RenameTable(
                name: "Score",
                newName: "Scores");

            migrationBuilder.RenameIndex(
                name: "IX_Score_UserId",
                table: "Scores",
                newName: "IX_Scores_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Score_SubmissionId1",
                table: "Scores",
                newName: "IX_Scores_SubmissionId1");

            migrationBuilder.RenameIndex(
                name: "IX_Score_SubmissionId_UserId",
                table: "Scores",
                newName: "IX_Scores_SubmissionId_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Score_LobbyId",
                table: "Scores",
                newName: "IX_Scores_LobbyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Scores",
                table: "Scores",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Scores_Lobbies_LobbyId",
                table: "Scores",
                column: "LobbyId",
                principalTable: "Lobbies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Scores_Submissions_SubmissionId",
                table: "Scores",
                column: "SubmissionId",
                principalTable: "Submissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Scores_Submissions_SubmissionId1",
                table: "Scores",
                column: "SubmissionId1",
                principalTable: "Submissions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Scores_Users_UserId",
                table: "Scores",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scores_Lobbies_LobbyId",
                table: "Scores");

            migrationBuilder.DropForeignKey(
                name: "FK_Scores_Submissions_SubmissionId",
                table: "Scores");

            migrationBuilder.DropForeignKey(
                name: "FK_Scores_Submissions_SubmissionId1",
                table: "Scores");

            migrationBuilder.DropForeignKey(
                name: "FK_Scores_Users_UserId",
                table: "Scores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Scores",
                table: "Scores");

            migrationBuilder.RenameTable(
                name: "Scores",
                newName: "Score");

            migrationBuilder.RenameIndex(
                name: "IX_Scores_UserId",
                table: "Score",
                newName: "IX_Score_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Scores_SubmissionId1",
                table: "Score",
                newName: "IX_Score_SubmissionId1");

            migrationBuilder.RenameIndex(
                name: "IX_Scores_SubmissionId_UserId",
                table: "Score",
                newName: "IX_Score_SubmissionId_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Scores_LobbyId",
                table: "Score",
                newName: "IX_Score_LobbyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Score",
                table: "Score",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Score_Lobbies_LobbyId",
                table: "Score",
                column: "LobbyId",
                principalTable: "Lobbies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Score_Submissions_SubmissionId",
                table: "Score",
                column: "SubmissionId",
                principalTable: "Submissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Score_Submissions_SubmissionId1",
                table: "Score",
                column: "SubmissionId1",
                principalTable: "Submissions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Score_Users_UserId",
                table: "Score",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
