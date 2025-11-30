using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class Mno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Books_Title_Category",
                table: "Books");

            migrationBuilder.CreateIndex(
                name: "IX_Books_Title_Category",
                table: "Books",
                columns: new[] { "Title", "Category" },
                unique: true,
                filter: "[Category] is not null");

            migrationBuilder.CreateIndex(
                name: "IX_Books_Title_NoCategory",
                table: "Books",
                column: "Title",
                unique: true,
                filter: "[Category] is null");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Books_Title_Category",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_Title_NoCategory",
                table: "Books");

            migrationBuilder.CreateIndex(
                name: "IX_Books_Title_Category",
                table: "Books",
                columns: new[] { "Title", "Category" },
                unique: true,
                filter: "[Category] IS NOT NULL");
        }
    }
}
