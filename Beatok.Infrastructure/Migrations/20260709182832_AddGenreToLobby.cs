using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatok.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGenreToLobby : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GenreId",
                table: "Lobbies",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Lobbies_GenreId",
                table: "Lobbies",
                column: "GenreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lobbies_Genres_GenreId",
                table: "Lobbies",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lobbies_Genres_GenreId",
                table: "Lobbies");

            migrationBuilder.DropIndex(
                name: "IX_Lobbies_GenreId",
                table: "Lobbies");

            migrationBuilder.DropColumn(
                name: "GenreId",
                table: "Lobbies");
        }
    }
}
