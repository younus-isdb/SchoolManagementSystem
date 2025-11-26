using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class Ban : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Class",
                table: "IssuedBooks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RollNumber",
                table: "IssuedBooks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Section",
                table: "IssuedBooks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserFullName",
                table: "IssuedBooks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserType",
                table: "IssuedBooks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Class",
                table: "IssuedBooks");

            migrationBuilder.DropColumn(
                name: "RollNumber",
                table: "IssuedBooks");

            migrationBuilder.DropColumn(
                name: "Section",
                table: "IssuedBooks");

            migrationBuilder.DropColumn(
                name: "UserFullName",
                table: "IssuedBooks");

            migrationBuilder.DropColumn(
                name: "UserType",
                table: "IssuedBooks");
        }
    }
}
