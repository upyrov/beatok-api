using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatok.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeGenreIdType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sounds_Categories_CategoryId1",
                table: "Sounds");

            migrationBuilder.DropIndex(
                name: "IX_Sounds_CategoryId1",
                table: "Sounds");

            migrationBuilder.DropColumn(
                name: "CategoryId1",
                table: "Sounds");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId1",
                table: "Sounds",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sounds_CategoryId1",
                table: "Sounds",
                column: "CategoryId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Sounds_Categories_CategoryId1",
                table: "Sounds",
                column: "CategoryId1",
                principalTable: "Categories",
                principalColumn: "Id");
        }
    }
}
