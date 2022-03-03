using Microsoft.EntityFrameworkCore.Migrations;

namespace FOODPEDI.API.REST.Migrations
{
    public partial class firstmigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IconeFolderPath",
                table: "Categories",
                newName: "IconFolderPath");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IconFolderPath",
                table: "Categories",
                newName: "IconeFolderPath");
        }
    }
}
