using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoodReads.Infrastructure.EntityFramework.Migrations.Books
{
    /// <inheritdoc />
    public partial class BookCoverTmpRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cover",
                schema: "books",
                table: "Books");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cover",
                schema: "books",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
