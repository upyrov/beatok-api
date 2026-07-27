using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatok.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLobbyTimestamps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubmissionTimeLimit",
                table: "Lobbies",
                newName: "SubmissionTime");

            migrationBuilder.RenameColumn(
                name: "Phase",
                table: "Lobbies",
                newName: "State");

            migrationBuilder.AlterColumn<string>(
                name: "JobId",
                table: "Lobbies",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndedAt",
                table: "Lobbies",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmissionStartedAt",
                table: "Lobbies",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "VotingStartedAt",
                table: "Lobbies",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndedAt",
                table: "Lobbies");

            migrationBuilder.DropColumn(
                name: "SubmissionStartedAt",
                table: "Lobbies");

            migrationBuilder.DropColumn(
                name: "VotingStartedAt",
                table: "Lobbies");

            migrationBuilder.RenameColumn(
                name: "SubmissionTime",
                table: "Lobbies",
                newName: "SubmissionTimeLimit");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "Lobbies",
                newName: "Phase");

            migrationBuilder.AlterColumn<string>(
                name: "JobId",
                table: "Lobbies",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
