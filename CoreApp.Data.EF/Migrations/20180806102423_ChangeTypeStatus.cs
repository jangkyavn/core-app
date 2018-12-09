using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreApp.Data.EF.Migrations
{
    public partial class ChangeTypeStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "Slides",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Slides",
                nullable: false,
                oldClrType: typeof(bool));
        }
    }
}
