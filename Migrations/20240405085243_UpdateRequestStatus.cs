using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRental_BE.Migrations
{
    public partial class UpdateRequestStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApprove",
                table: "ApprovalApplications");

            migrationBuilder.AddColumn<int>(
                name: "RequestStatus",
                table: "ApprovalApplications",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestStatus",
                table: "ApprovalApplications");

            migrationBuilder.AddColumn<bool>(
                name: "IsApprove",
                table: "ApprovalApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
