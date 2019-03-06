using Microsoft.EntityFrameworkCore.Migrations;

namespace EpMon.Data.Migrations
{
    public partial class ExtendEndpoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Endpoints",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Endpoints",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Endpoints");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Endpoints");
        }
    }
}
