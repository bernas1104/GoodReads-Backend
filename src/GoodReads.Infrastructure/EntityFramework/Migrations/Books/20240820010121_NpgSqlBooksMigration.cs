using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GoodReads.Infrastructure.EntityFramework.Migrations.Books
{
    /// <inheritdoc />
    public partial class NpgSqlBooksMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "books");

            migrationBuilder.CreateTable(
                name: "Books",
                schema: "books",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ISBN = table.Column<string>(type: "text", nullable: false),
                    Author = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookDatas",
                schema: "books",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Publisher = table.Column<string>(type: "text", nullable: false),
                    YearOfPublication = table.Column<int>(type: "integer", nullable: false),
                    Pages = table.Column<int>(type: "integer", nullable: false),
                    BookId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookDatas_Books_BookId",
                        column: x => x.BookId,
                        principalSchema: "books",
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookRatingIds",
                schema: "books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BookRatingId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookRatingIds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookRatingIds_Books_BookId",
                        column: x => x.BookId,
                        principalSchema: "books",
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeanScores",
                schema: "books",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Scores = table.Column<string>(type: "text", nullable: false),
                    BookId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeanScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeanScores_Books_BookId",
                        column: x => x.BookId,
                        principalSchema: "books",
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookDatas_BookId",
                schema: "books",
                table: "BookDatas",
                column: "BookId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookRatingIds_BookId",
                schema: "books",
                table: "BookRatingIds",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_ISBN",
                schema: "books",
                table: "Books",
                column: "ISBN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MeanScores_BookId",
                schema: "books",
                table: "MeanScores",
                column: "BookId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookDatas",
                schema: "books");

            migrationBuilder.DropTable(
                name: "BookRatingIds",
                schema: "books");

            migrationBuilder.DropTable(
                name: "MeanScores",
                schema: "books");

            migrationBuilder.DropTable(
                name: "Books",
                schema: "books");
        }
    }
}
