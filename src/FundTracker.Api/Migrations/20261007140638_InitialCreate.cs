using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FundTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Funds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Isin = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NavPrices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FundId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NavPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NavPrices_Funds_FundId",
                        column: x => x.FundId,
                        principalTable: "Funds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FundId = table.Column<int>(type: "int", nullable: false),
                    InvestorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TradeDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Funds_FundId",
                        column: x => x.FundId,
                        principalTable: "Funds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "Id", "Currency", "Isin", "Name" },
                values: new object[,]
                {
                    { 1, "EUR", "LU2000000013", "Euro Equity Growth" },
                    { 2, "EUR", "LU2000000021", "Global Bond Income" },
                    { 3, "USD", "LU2000000039", "US Tech Leaders" }
                });

            migrationBuilder.InsertData(
                table: "NavPrices",
                columns: new[] { "Id", "Date", "FundId", "Value" },
                values: new object[,]
                {
                    { 1, new DateOnly(2026, 9, 21), 1, 100.00m },
                    { 2, new DateOnly(2026, 9, 22), 1, 100.75m },
                    { 3, new DateOnly(2026, 9, 23), 1, 101.50m },
                    { 4, new DateOnly(2026, 9, 24), 1, 102.25m },
                    { 5, new DateOnly(2026, 9, 25), 1, 103.00m },
                    { 6, new DateOnly(2026, 9, 26), 1, 103.75m },
                    { 7, new DateOnly(2026, 9, 27), 1, 104.50m },
                    { 8, new DateOnly(2026, 9, 28), 1, 105.25m },
                    { 9, new DateOnly(2026, 9, 29), 1, 106.00m },
                    { 10, new DateOnly(2026, 9, 30), 1, 106.75m },
                    { 11, new DateOnly(2026, 9, 21), 2, 250.00m },
                    { 12, new DateOnly(2026, 9, 22), 2, 250.75m },
                    { 13, new DateOnly(2026, 9, 23), 2, 251.50m },
                    { 14, new DateOnly(2026, 9, 24), 2, 252.25m },
                    { 15, new DateOnly(2026, 9, 25), 2, 253.00m },
                    { 16, new DateOnly(2026, 9, 26), 2, 253.75m },
                    { 17, new DateOnly(2026, 9, 27), 2, 254.50m },
                    { 18, new DateOnly(2026, 9, 28), 2, 255.25m },
                    { 19, new DateOnly(2026, 9, 29), 2, 256.00m },
                    { 20, new DateOnly(2026, 9, 30), 2, 256.75m },
                    { 21, new DateOnly(2026, 9, 21), 3, 80.00m },
                    { 22, new DateOnly(2026, 9, 22), 3, 80.75m },
                    { 23, new DateOnly(2026, 9, 23), 3, 81.50m },
                    { 24, new DateOnly(2026, 9, 24), 3, 82.25m },
                    { 25, new DateOnly(2026, 9, 25), 3, 83.00m },
                    { 26, new DateOnly(2026, 9, 26), 3, 83.75m },
                    { 27, new DateOnly(2026, 9, 27), 3, 84.50m },
                    { 28, new DateOnly(2026, 9, 28), 3, 85.25m },
                    { 29, new DateOnly(2026, 9, 29), 3, 86.00m },
                    { 30, new DateOnly(2026, 9, 30), 3, 86.75m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Funds_Isin",
                table: "Funds",
                column: "Isin",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NavPrices_FundId_Date",
                table: "NavPrices",
                columns: new[] { "FundId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ExternalId",
                table: "Transactions",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_FundId_TradeDate",
                table: "Transactions",
                columns: new[] { "FundId", "TradeDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NavPrices");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Funds");
        }
    }
}
