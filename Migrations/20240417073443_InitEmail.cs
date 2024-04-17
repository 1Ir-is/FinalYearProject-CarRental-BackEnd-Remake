using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRental_BE.Migrations
{
    public partial class InitEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResetKey",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetKeyTimestamp",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "ResetKey",
                value: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetKey",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResetKeyTimestamp",
                table: "Users");
        }
    }
}
