using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Summaries.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBookPricingAndPurchases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PdfUrl",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PriceKobo",
                table: "Books",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Purchases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AmountKobo = table.Column<long>(type: "bigint", nullable: false),
                    PaystackReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CompletedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_PaystackReference",
                table: "Purchases",
                column: "PaystackReference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_UserId_BookId",
                table: "Purchases",
                columns: new[] { "UserId", "BookId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Purchases");

            migrationBuilder.DropColumn(
                name: "PdfUrl",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "PriceKobo",
                table: "Books");
        }
    }
}
