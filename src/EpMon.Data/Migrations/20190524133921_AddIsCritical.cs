using Microsoft.EntityFrameworkCore.Migrations;

namespace EpMon.Data.Migrations
{
    public partial class AddIsCritical : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCritical",
                table: "Endpoints",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCritical",
                table: "Endpoints");
        }
    }
}
