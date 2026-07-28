using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatok.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLobbySubmissionsAndSoundsConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "IX_Sounds_LobbyId",
                table: "Sounds");

            migrationBuilder.DropIndex(
                name: "IX_Lobbies_WinningSubmissionId",
                table: "Lobbies");

            migrationBuilder.DropColumn(
                name: "LobbyId",
                table: "Sounds");

            migrationBuilder.AlterColumn<Guid>(
                name: "LobbyId",
                table: "Submissions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "LobbySound",
                columns: table => new
                {
                    LobbyId = table.Column<Guid>(type: "uuid", nullable: false),
                    SoundsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LobbySound", x => new { x.LobbyId, x.SoundsId });
                    table.ForeignKey(
                        name: "FK_LobbySound_Lobbies_LobbyId",
                        column: x => x.LobbyId,
                        principalTable: "Lobbies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LobbySound_Sounds_SoundsId",
                        column: x => x.SoundsId,
                        principalTable: "Sounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LobbySound_SoundsId",
                table: "LobbySound",
                column: "SoundsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Lobbies_LobbyId",
                table: "Submissions",
                column: "LobbyId",
                principalTable: "Lobbies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Lobbies_LobbyId",
                table: "Submissions");

            migrationBuilder.DropTable(
                name: "LobbySound");

            migrationBuilder.AlterColumn<Guid>(
                name: "LobbyId",
                table: "Submissions",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "LobbyId",
                table: "Sounds",
                type: "uuid",
                nullable: true);

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
    }
}
