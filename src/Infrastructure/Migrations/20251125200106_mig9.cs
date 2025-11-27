using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mig9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedUserId",
                table: "WorkOrders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedUserId",
                table: "WorkOrderParts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedUserId",
                table: "Vehicles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedUserId",
                table: "Stocks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedUserId",
                table: "StockPriceTypes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedUserId",
                table: "StockPrices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedUserId",
                table: "StockGroups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedUserId",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedUserId",
                table: "InvoiceItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedUserId",
                table: "Inventories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedUserId",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedUserId",
                table: "Depots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedUserId",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedUserId",
                table: "Acts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedUserId",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "CreatedUserId",
                table: "WorkOrderParts");

            migrationBuilder.DropColumn(
                name: "CreatedUserId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "CreatedUserId",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "CreatedUserId",
                table: "StockPriceTypes");

            migrationBuilder.DropColumn(
                name: "CreatedUserId",
                table: "StockPrices");

            migrationBuilder.DropColumn(
                name: "CreatedUserId",
                table: "StockGroups");

            migrationBuilder.DropColumn(
                name: "CreatedUserId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CreatedUserId",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "CreatedUserId",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "CreatedUserId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CreatedUserId",
                table: "Depots");

            migrationBuilder.DropColumn(
                name: "CreatedUserId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "CreatedUserId",
                table: "Acts");
        }
    }
}
