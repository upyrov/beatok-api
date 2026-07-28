using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatok.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSoundsAndSubmissionToLobby : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LobbyId",
                table: "Submissions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LobbyId",
                table: "Sounds",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WinningSubmissionId",
                table: "Lobbies",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_LobbyId",
                table: "Submissions",
                column: "LobbyId");

            migrationBuilder.CreateIndex(
                name: "IX_Sounds_LobbyId",
                table: "Sounds",
                column: "LobbyId");

            migrationBuilder.CreateIndex(
                name: "IX_Lobbies_WinningSubmissionId",
                table: "Lobbies",
                column: "WinningSubmissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lobbies_Submissions_WinningSubmissionId",
                table: "Lobbies",
                column: "WinningSubmissionId",
                principalTable: "Submissions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sounds_Lobbies_LobbyId",
                table: "Sounds",
                column: "LobbyId",
                principalTable: "Lobbies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Lobbies_LobbyId",
                table: "Submissions",
                column: "LobbyId",
                principalTable: "Lobbies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lobbies_Submissions_WinningSubmissionId",
                table: "Lobbies");

            migrationBuilder.DropForeignKey(
                name: "FK_Sounds_Lobbies_LobbyId",
                table: "Sounds");

            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Lobbies_LobbyId",
                table: "Submissions");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_LobbyId",
                table: "Submissions");

            migrationBuilder.DropIndex(
                name: "IX_Sounds_LobbyId",
                table: "Sounds");

            migrationBuilder.DropIndex(
                name: "IX_Lobbies_WinningSubmissionId",
                table: "Lobbies");

            migrationBuilder.DropColumn(
                name: "LobbyId",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "LobbyId",
                table: "Sounds");

            migrationBuilder.DropColumn(
                name: "WinningSubmissionId",
                table: "Lobbies");
        }
    }
}
