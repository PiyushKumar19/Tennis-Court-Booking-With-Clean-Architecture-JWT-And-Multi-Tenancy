using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TennisCourtBookings.Persistence.Migrations
{
    public partial class AzureCommitAddedpasswordproptoUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Token",
                table: "Users",
                newName: "Password");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Users",
                newName: "Token");
        }
    }
}
