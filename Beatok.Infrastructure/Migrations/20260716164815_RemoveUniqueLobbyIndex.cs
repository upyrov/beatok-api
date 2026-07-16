using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatok.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueLobbyIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Participation_UserId_LobbyId",
                table: "Participation");

            migrationBuilder.CreateIndex(
                name: "IX_Participation_UserId",
                table: "Participation",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Participation_UserId",
                table: "Participation");

            migrationBuilder.CreateIndex(
                name: "IX_Participation_UserId_LobbyId",
                table: "Participation",
                columns: new[] { "UserId", "LobbyId" },
                unique: true);
        }
    }
}
