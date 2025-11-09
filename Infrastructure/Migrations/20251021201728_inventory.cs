using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class inventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Inventories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InvoiceId",
                table: "Inventories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsInput",
                table: "Inventories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "WorkOrderId",
                table: "Inventories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_InvoiceId",
                table: "Inventories",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_WorkOrderId",
                table: "Inventories",
                column: "WorkOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_Invoices_InvoiceId",
                table: "Inventories",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_WorkOrders_WorkOrderId",
                table: "Inventories",
                column: "WorkOrderId",
                principalTable: "WorkOrders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_Invoices_InvoiceId",
                table: "Inventories");

            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_WorkOrders_WorkOrderId",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_InvoiceId",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_WorkOrderId",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "IsInput",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "WorkOrderId",
                table: "Inventories");
        }
    }
}
