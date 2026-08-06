using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatok.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    IsAnonymous = table.Column<bool>(type: "boolean", nullable: false),
                    Picture = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    LastActiveAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Mu = table.Column<double>(type: "double precision", nullable: false),
                    Sigma = table.Column<double>(type: "double precision", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    KitId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Kits_KitId",
                        column: x => x.KitId,
                        principalTable: "Kits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenreKit",
                columns: table => new
                {
                    GenreId = table.Column<Guid>(type: "uuid", nullable: false),
                    KitId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreKit", x => new { x.GenreId, x.KitId });
                    table.ForeignKey(
                        name: "FK_GenreKit_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenreKit_Kits_KitId",
                        column: x => x.KitId,
                        principalTable: "Kits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<string>(type: "text", nullable: false),
                    TargetUserId = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Users_TargetUserId",
                        column: x => x.TargetUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lobbies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ParticipantLimit = table.Column<short>(type: "smallint", nullable: false),
                    JobId = table.Column<string>(type: "text", nullable: true),
                    SubmissionTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    VotingTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    State = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SubmissionStartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VotingStartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GenreId = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<string>(type: "text", nullable: false),
                    WinningSubmissionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lobbies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lobbies_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Lobbies_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sounds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sounds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sounds_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Participation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsConnected = table.Column<bool>(type: "boolean", nullable: false),
                    IsKicked = table.Column<bool>(type: "boolean", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ConnectionId = table.Column<string>(type: "text", nullable: true),
                    DisconnectJobId = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LobbyId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Participation_Lobbies_LobbyId",
                        column: x => x.LobbyId,
                        principalTable: "Lobbies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Participation_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateTable(
                name: "Submissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    DurationSeconds = table.Column<int>(type: "integer", nullable: false),
                    ParticipationId = table.Column<Guid>(type: "uuid", nullable: false),
                    LobbyId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Submissions_Lobbies_LobbyId",
                        column: x => x.LobbyId,
                        principalTable: "Lobbies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Submissions_Participation_ParticipationId",
                        column: x => x.ParticipationId,
                        principalTable: "Participation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Scores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<short>(type: "smallint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LobbyId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParticipationId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubmissionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scores_Lobbies_LobbyId",
                        column: x => x.LobbyId,
                        principalTable: "Lobbies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Scores_Participation_ParticipationId",
                        column: x => x.ParticipationId,
                        principalTable: "Participation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Scores_Submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_KitId",
                table: "Categories",
                column: "KitId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AuthorId",
                table: "Comments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_TargetUserId",
                table: "Comments",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GenreKit_KitId_GenreId",
                table: "GenreKit",
                columns: new[] { "KitId", "GenreId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lobbies_GenreId",
                table: "Lobbies",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Lobbies_OwnerId",
                table: "Lobbies",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_LobbySound_SoundsId",
                table: "LobbySound",
                column: "SoundsId");

            migrationBuilder.CreateIndex(
                name: "IX_Participation_ConnectionId",
                table: "Participation",
                column: "ConnectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Participation_LobbyId",
                table: "Participation",
                column: "LobbyId");

            migrationBuilder.CreateIndex(
                name: "IX_Participation_UserId",
                table: "Participation",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Scores_LobbyId",
                table: "Scores",
                column: "LobbyId");

            migrationBuilder.CreateIndex(
                name: "IX_Scores_ParticipationId",
                table: "Scores",
                column: "ParticipationId");

            migrationBuilder.CreateIndex(
                name: "IX_Scores_SubmissionId_ParticipationId",
                table: "Scores",
                columns: new[] { "SubmissionId", "ParticipationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sounds_CategoryId",
                table: "Sounds",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_LobbyId",
                table: "Submissions",
                column: "LobbyId");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_ParticipationId",
                table: "Submissions",
                column: "ParticipationId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "GenreKit");

            migrationBuilder.DropTable(
                name: "LobbySound");

            migrationBuilder.DropTable(
                name: "Scores");

            migrationBuilder.DropTable(
                name: "Sounds");

            migrationBuilder.DropTable(
                name: "Submissions");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Participation");

            migrationBuilder.DropTable(
                name: "Kits");

            migrationBuilder.DropTable(
                name: "Lobbies");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
