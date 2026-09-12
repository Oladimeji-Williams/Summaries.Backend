using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Summaries.Infrastructure.Identity.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailSignIn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EmailSignInEnabled",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "EmailSignInAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CodeHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConsumedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailedAttempts = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailSignInAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailSignInAttempts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailSignInAttempts_TokenHash",
                table: "EmailSignInAttempts",
                column: "TokenHash");

            migrationBuilder.CreateIndex(
                name: "IX_EmailSignInAttempts_UserId_ConsumedAtUtc",
                table: "EmailSignInAttempts",
                columns: new[] { "UserId", "ConsumedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailSignInAttempts");

            migrationBuilder.DropColumn(
                name: "EmailSignInEnabled",
                table: "AspNetUsers");
        }
    }
}
