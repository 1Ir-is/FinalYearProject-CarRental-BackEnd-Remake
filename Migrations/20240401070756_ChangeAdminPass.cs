using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRental_BE.Migrations
{
    public partial class ChangeAdminPass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Password",
                value: "bLuioEpOf4gkryRaCzveGZuEnH8GZ6Na");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Password",
                value: "i2yBMU+FxDo=");
        }
    }
}
