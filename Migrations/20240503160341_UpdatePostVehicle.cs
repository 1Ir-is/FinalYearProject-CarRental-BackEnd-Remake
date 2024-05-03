using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRental_BE.Migrations
{
    public partial class UpdatePostVehicle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRented",
                table: "PostVehicles",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRented",
                table: "PostVehicles");
        }
    }
}
