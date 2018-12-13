using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreApp.Data.EF.Migrations
{
    public partial class AddColumnType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "decimal(18,2)",
                table: "WholePrices",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "decimal(18,2)",
                table: "SystemConfigs",
                newName: "Value5");

            migrationBuilder.RenameColumn(
                name: "decimal(18,2)",
                table: "Products",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "decimal(18,2)",
                table: "BillDetails",
                newName: "Price");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "WholePrices",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "Value5",
                table: "SystemConfigs",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PromotionPrice",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "OriginalPrice",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "BillDetails",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "WholePrices",
                newName: "decimal(18,2)");

            migrationBuilder.RenameColumn(
                name: "Value5",
                table: "SystemConfigs",
                newName: "decimal(18,2)");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Products",
                newName: "decimal(18,2)");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "BillDetails",
                newName: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "decimal(18,2)",
                table: "WholePrices",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "decimal(18,2)",
                table: "SystemConfigs",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PromotionPrice",
                table: "Products",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "OriginalPrice",
                table: "Products",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "decimal(18,2)",
                table: "Products",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "decimal(18,2)",
                table: "BillDetails",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
