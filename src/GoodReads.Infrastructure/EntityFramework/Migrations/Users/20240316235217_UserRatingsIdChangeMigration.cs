using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoodReads.Infrastructure.EntityFramework.Migrations.Users
{
    /// <inheritdoc />
    public partial class UserRatingsIdChangeMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RatingId",
                schema: "users",
                table: "UserRatingIds",
                newName: "UserRatingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserRatingId",
                schema: "users",
                table: "UserRatingIds",
                newName: "RatingId");
        }
    }
}
