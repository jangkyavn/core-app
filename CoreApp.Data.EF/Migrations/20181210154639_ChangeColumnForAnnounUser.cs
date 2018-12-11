using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreApp.Data.EF.Migrations
{
    public partial class ChangeColumnForAnnounUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AnnouncementUsers",
                newName: "ReaderId");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AnnouncementUsers",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AnnouncementUsers");

            migrationBuilder.RenameColumn(
                name: "ReaderId",
                table: "AnnouncementUsers",
                newName: "UserId");
        }
    }
}
