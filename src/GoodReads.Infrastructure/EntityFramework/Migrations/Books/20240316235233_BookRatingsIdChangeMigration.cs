using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoodReads.Infrastructure.EntityFramework.Migrations.Books
{
    /// <inheritdoc />
    public partial class BookRatingsIdChangeMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RatingId",
                schema: "books",
                table: "BookRatingIds",
                newName: "BookRatingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BookRatingId",
                schema: "books",
                table: "BookRatingIds",
                newName: "RatingId");
        }
    }
}
