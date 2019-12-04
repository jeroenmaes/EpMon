using Microsoft.EntityFrameworkCore.Migrations;

namespace EpMon.Data.Migrations
{
    public partial class AddEndpointPublishStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PublishStats",
                table: "Endpoints",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublishStats",
                table: "Endpoints");
        }
    }
}
