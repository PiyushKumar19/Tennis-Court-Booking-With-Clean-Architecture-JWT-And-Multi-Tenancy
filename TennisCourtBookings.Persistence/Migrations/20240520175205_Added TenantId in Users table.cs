using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TennisCourtBookings.Persistence.Migrations
{
    public partial class AddedTenantIdinUserstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Users");
        }
    }
}
