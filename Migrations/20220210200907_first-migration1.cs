using Microsoft.EntityFrameworkCore.Migrations;

namespace FOODPEDI.API.REST.Migrations
{
    public partial class firstmigration1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImagePath4",
                table: "Categories",
                newName: "ImageId2");

            migrationBuilder.RenameColumn(
                name: "ImagePath3",
                table: "Categories",
                newName: "ImageId1");

            migrationBuilder.RenameColumn(
                name: "ImagePath2",
                table: "Categories",
                newName: "ImageFolderPath");

            migrationBuilder.RenameColumn(
                name: "ImagePath1",
                table: "Categories",
                newName: "IconeFolderPath");

            migrationBuilder.AlterColumn<string>(
                name: "ParentId",
                table: "Categories",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IconId1",
                table: "Categories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IconId2",
                table: "Categories",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconId1",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IconId2",
                table: "Categories");

            migrationBuilder.RenameColumn(
                name: "ImageId2",
                table: "Categories",
                newName: "ImagePath4");

            migrationBuilder.RenameColumn(
                name: "ImageId1",
                table: "Categories",
                newName: "ImagePath3");

            migrationBuilder.RenameColumn(
                name: "ImageFolderPath",
                table: "Categories",
                newName: "ImagePath2");

            migrationBuilder.RenameColumn(
                name: "IconeFolderPath",
                table: "Categories",
                newName: "ImagePath1");

            migrationBuilder.AlterColumn<int>(
                name: "ParentId",
                table: "Categories",
                type: "integer",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
