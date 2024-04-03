using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmazonScrapeSelenium.Migrations
{
    public partial class commenterAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVisited",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVisited",
                table: "Products");
        }
    }
}
