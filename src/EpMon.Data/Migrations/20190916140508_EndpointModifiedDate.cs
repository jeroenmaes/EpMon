using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EpMon.Data.Migrations
{
    public partial class EndpointModifiedDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "Endpoints",
                nullable: false,
                defaultValue: DateTime.UtcNow);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDateTime",
                table: "Endpoints",
                nullable: false,
                defaultValue: DateTime.UtcNow);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "Endpoints");

            migrationBuilder.DropColumn(
                name: "ModifiedDateTime",
                table: "Endpoints");
        }
    }
}
