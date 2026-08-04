using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatok.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsKicked : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsKicked",
                table: "Participation",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsKicked",
                table: "Participation");
        }
    }
}
