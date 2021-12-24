using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace eBroker.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Equity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquityName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EquityDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EquityTransaction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    EquityId = table.Column<int>(type: "int", nullable: false),
                    Units = table.Column<int>(type: "int", nullable: false),
                    PricePerUnit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquityTransaction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EquityHolding",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    EquityId = table.Column<int>(type: "int", nullable: false),
                    Units = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquityHolding", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquityHolding_Equity_EquityId",
                        column: x => x.EquityId,
                        principalTable: "Equity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Account",
                columns: new[] { "Id", "Balance", "CreatedOn", "CustomerId", "CustomerName", "IsActive", "ModifiedOn" },
                values: new object[,]
                {
                    { 1, 0m, new DateTime(2021, 12, 24, 12, 49, 5, 815, DateTimeKind.Local).AddTicks(315), 1, "Shashank", true, new DateTime(2021, 12, 24, 12, 49, 5, 816, DateTimeKind.Local).AddTicks(1680) },
                    { 2, 100m, new DateTime(2021, 12, 24, 12, 49, 5, 816, DateTimeKind.Local).AddTicks(2140), 2, "Test User 1", true, new DateTime(2021, 12, 24, 12, 49, 5, 816, DateTimeKind.Local).AddTicks(2145) },
                    { 3, 3000m, new DateTime(2021, 12, 24, 12, 49, 5, 816, DateTimeKind.Local).AddTicks(2148), 3, "Test User 2", true, new DateTime(2021, 12, 24, 12, 49, 5, 816, DateTimeKind.Local).AddTicks(2149) }
                });

            migrationBuilder.InsertData(
                table: "Equity",
                columns: new[] { "Id", "CreatedOn", "EquityDescription", "EquityName", "ModifiedOn" },
                values: new object[,]
                {
                    { 1, new DateTime(2021, 12, 24, 12, 49, 5, 817, DateTimeKind.Local).AddTicks(4700), "Equity 1", "Equity 1", new DateTime(2021, 12, 24, 12, 49, 5, 817, DateTimeKind.Local).AddTicks(4709) },
                    { 2, new DateTime(2021, 12, 24, 12, 49, 5, 817, DateTimeKind.Local).AddTicks(4714), "Equity 2", "Equity 2", new DateTime(2021, 12, 24, 12, 49, 5, 817, DateTimeKind.Local).AddTicks(4715) },
                    { 3, new DateTime(2021, 12, 24, 12, 49, 5, 817, DateTimeKind.Local).AddTicks(4717), "Equity 3", "Equity 3", new DateTime(2021, 12, 24, 12, 49, 5, 817, DateTimeKind.Local).AddTicks(4718) }
                });

            migrationBuilder.InsertData(
                table: "EquityHolding",
                columns: new[] { "Id", "CreatedOn", "CustomerId", "EquityId", "ModifiedOn", "Units" },
                values: new object[,]
                {
                    { 1, new DateTime(2021, 12, 24, 12, 49, 5, 817, DateTimeKind.Local).AddTicks(6172), 1, 1, new DateTime(2021, 12, 24, 12, 49, 5, 817, DateTimeKind.Local).AddTicks(6177), 1000 },
                    { 2, new DateTime(2021, 12, 24, 12, 49, 5, 817, DateTimeKind.Local).AddTicks(6182), 2, 1, new DateTime(2021, 12, 24, 12, 49, 5, 817, DateTimeKind.Local).AddTicks(6183), 20 },
                    { 3, new DateTime(2021, 12, 24, 12, 49, 5, 817, DateTimeKind.Local).AddTicks(6185), 2, 2, new DateTime(2021, 12, 24, 12, 49, 5, 817, DateTimeKind.Local).AddTicks(6186), 30 },
                    { 4, new DateTime(2021, 12, 24, 12, 49, 5, 817, DateTimeKind.Local).AddTicks(6187), 2, 3, new DateTime(2021, 12, 24, 12, 49, 5, 817, DateTimeKind.Local).AddTicks(6189), 100 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquityHolding_EquityId",
                table: "EquityHolding",
                column: "EquityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "EquityHolding");

            migrationBuilder.DropTable(
                name: "EquityTransaction");

            migrationBuilder.DropTable(
                name: "Equity");
        }
    }
}
