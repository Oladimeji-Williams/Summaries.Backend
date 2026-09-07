using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Summaries.Infrastructure.Identity.Migrations
{
    /// <inheritdoc />
    public partial class AddLoginNotificationThrottle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginNotificationSentAtUtc",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastLoginNotificationSentAtUtc",
                table: "AspNetUsers");
        }
    }
}
