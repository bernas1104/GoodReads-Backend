using System.Diagnostics.CodeAnalysis;

using Bogus;

using GoodReads.Application.Features.Books;
using GoodReads.Application.Features.Books.Create;
using GoodReads.Application.Features.Books.GetById;
using GoodReads.Application.Features.Books.GetPaginated;
using GoodReads.Application.Features.Books.Update;
using GoodReads.Domain.BookAggregate.Builders;
using GoodReads.Domain.BookAggregate.Entities;
using GoodReads.Domain.BookAggregate.Enums;

namespace GoodReads.Shared.Mocks
{
    [ExcludeFromCodeCoverage]
    public static class BookMock
    {
        public static Book Get(
            string? title = null,
            string? isbn = null,
            string? author = null,
            Gender? gender = null
        )
        {
            return new Faker<Book>().CustomInstantiator(
                f =>
                {
                    var builder = new BookBuilder(
                        title: title ?? f.Random.String2(10),
                        isbn: isbn ?? f.Random.String2(20),
                        author: author ?? f.Person.FullName,
                        gender: gender ?? Gender.FromValue(f.Random.Int(0, 5))
                    );

                    builder.AddDescription(f.Random.String2(20));
                    builder.AddBookData(
                        f.Company.CompanyName(),
                        f.Date.Recent().Year,
                        f.Random.Int(100, 500)
                    );

                    return builder.GetBook();
                }
            );
        }

        public static IEnumerable<Book> GetBooks(int? quantity = null)
        {
            return new Faker<Book>().CustomInstantiator(f => {
                var builder = new BookBuilder(
                        title: f.Random.String2(10),
                        isbn: f.Random.String2(20),
                        author: f.Person.FullName,
                        gender: Gender.FromValue(f.Random.Int(0, 5))
                    );

                    builder.AddDescription(f.Random.String2(20));
                    builder.AddBookData(
                        f.Company.CompanyName(),
                        f.Date.Recent().Year,
                        f.Random.Int(100, 500)
                    );

                    return builder.GetBook();
            }).GenerateBetween(quantity ?? 1, quantity ?? 1);
        }

        public static CreateBookRequest GetCreateBookRequest(string? isbn = null)
        {
            return new Faker<CreateBookRequest>().CustomInstantiator(f => (
                new CreateBookRequest(
                    Title: f.Random.String2(10),
                    Description: f.Random.String2(20),
                    Isbn: isbn ?? f.Random.String2(10),
                    Author: f.Person.FullName,
                    Gender: f.Random.Int(0, 5),
                    BookData: new BookDataRequest
                    (
                        Publisher: f.Company.CompanyName(),
                        YearOfPublication: f.Random.Int(1900, DateTime.UtcNow.Year),
                        Pages: f.Random.Int(20, 500)
                    )
                )
            ));
        }

        public static GetPaginatedBooksRequest GetPaginatedBooksRequest(
            int? page = null,
            int? size = null
        )
        {
            return new Faker<GetPaginatedBooksRequest>().CustomInstantiator(f => (
                new GetPaginatedBooksRequest(
                    Page: page ?? f.Random.Int(1, 10),
                    Size: size ?? f.Random.Int(10, 20)
                )
            ));
        }

        public static Faker<BookResponse> GetFakeBookResponse()
        {
            return new Faker<BookResponse>().CustomInstantiator(f => (
                new BookResponse(
                    Title: f.Random.String2(10),
                    Isbn: f.Random.String2(10),
                    Author: f.Person.FullName,
                    Gender: Gender.FromValue(f.Random.Int(0, 5)).Name
                )
            ));
        }

        public static GetBookByIdResponse GetBookByIdResponse()
        {
            return new Faker<GetBookByIdResponse>().CustomInstantiator(f => (
                new GetBookByIdResponse(
                    Title: f.Random.String2(10),
                    Description: f.Random.String2(20),
                    Isbn: f.Random.String2(10),
                    Author: f.Person.FullName,
                    MeanScore: f.Random.Decimal(0, 5),
                    Gender: Gender.FromValue(f.Random.Int(0, 5)).Name,
                    BookData: GetBookDataResponse(),
                    RatingIds: new List<Guid>{ Guid.NewGuid() }
                )
            ));
        }

        public static BookDataResponse GetBookDataResponse()
        {
            return new Faker<BookDataResponse>().CustomInstantiator(f => (
                new BookDataResponse(
                    Publisher: f.Company.CompanyName(),
                    YearOfPublication: f.Random.Int(1900, DateTime.UtcNow.Year),
                    Pages: f.Random.Int(20, 500)
                )
            ));
        }

        public static UpdateBookRequest GetUpdateBookRequest(
            Guid? id = null,
            string? description = null
        )
        {
            return new Faker<UpdateBookRequest>().CustomInstantiator(f => (
                new UpdateBookRequest(
                    Id: id ?? Guid.NewGuid(),
                    Description: description ?? f.Random.String2(20)
                )
            ));
        }
    }
}