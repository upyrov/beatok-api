using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatok.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConnectionId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Participation_ConnectionId",
                table: "Participation");

            migrationBuilder.CreateIndex(
                name: "IX_Participation_ConnectionId",
                table: "Participation",
                column: "ConnectionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Participation_ConnectionId",
                table: "Participation");

            migrationBuilder.CreateIndex(
                name: "IX_Participation_ConnectionId",
                table: "Participation",
                column: "ConnectionId",
                unique: true);
        }
    }
}
