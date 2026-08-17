using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatok.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLobbyPlaybackItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VotingTime",
                table: "Lobbies");

            migrationBuilder.CreateTable(
                name: "LobbyPlaybackItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LobbyId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubmissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LobbyPlaybackItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LobbyPlaybackItems_Lobbies_LobbyId",
                        column: x => x.LobbyId,
                        principalTable: "Lobbies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LobbyPlaybackItems_Submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LobbyPlaybackItems_LobbyId",
                table: "LobbyPlaybackItems",
                column: "LobbyId");

            migrationBuilder.CreateIndex(
                name: "IX_LobbyPlaybackItems_SubmissionId",
                table: "LobbyPlaybackItems",
                column: "SubmissionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LobbyPlaybackItems");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "VotingTime",
                table: "Lobbies",
                type: "interval",
                nullable: true);
        }
    }
}
