using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmazonScrapeSelenium.Migrations
{
    public partial class CommentersAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommenterId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Commenters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Profilelink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commenters", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CommenterId",
                table: "Products",
                column: "CommenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Commenters_CommenterId",
                table: "Products",
                column: "CommenterId",
                principalTable: "Commenters",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Commenters_CommenterId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Commenters");

            migrationBuilder.DropIndex(
                name: "IX_Products_CommenterId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CommenterId",
                table: "Products");
        }
    }
}
