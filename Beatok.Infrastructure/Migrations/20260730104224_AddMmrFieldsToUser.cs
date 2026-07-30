using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatok.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMmrFieldsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Mu",
                table: "Users",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Sigma",
                table: "Users",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mu",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Sigma",
                table: "Users");
        }
    }
}
