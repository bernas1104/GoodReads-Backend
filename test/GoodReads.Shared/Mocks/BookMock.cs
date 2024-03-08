using System.Diagnostics.CodeAnalysis;

using Bogus;

using GoodReads.Application.Features.Books;
using GoodReads.Application.Features.Books.Create;
using GoodReads.Application.Features.Books.GetById;
using GoodReads.Application.Features.Books.GetPaginated;
using GoodReads.Application.Features.Books.Update;
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
            return new Faker<Book>().CustomInstantiator(f =>
                Book.Create(
                    title: title ?? f.Random.String2(10),
                    isbn: isbn ?? f.Random.String2(20),
                    author: author ?? f.Person.FullName,
                    gender: gender ?? Gender.FromValue(f.Random.Int(0, 5))
                )
            );
        }

        public static CreateBookRequest GetCreateBookRequest()
        {
            return new Faker<CreateBookRequest>().CustomInstantiator(f => (
                new CreateBookRequest(
                    Title: f.Random.String2(10),
                    Description: f.Random.String2(20),
                    Isbn: f.Random.String2(10),
                    Author: f.Person.FullName,
                    Gender: f.Random.Int(0, 5),
                    BookData: new BookDataRequest
                    (
                        Publisher: f.Company.CompanyName(),
                        YearOfPublication: f.Random.Int(1900, DateTime.UtcNow.Year),
                        Pages: f.Random.Int(20, 500)
                    ),
                    Cover: Array.Empty<byte>()
                )
            ));
        }

        public static GetPaginatedBooksResponse GetPaginatedBooksResponse(
            int? size = null
        )
        {
            return new Faker<GetPaginatedBooksResponse>().CustomInstantiator(f => (
                new GetPaginatedBooksResponse(
                    Data: GetBookResponse().GenerateBetween(size ?? 10, size ?? 10),
                    CurrentPage: f.Random.Int(1, 10),
                    TotalItens: f.Random.Int(10, 20),
                    TotalPages: f.Random.Int(1, 2),
                    PageSize: size ?? f.Random.Int(5, 10)
                )
            ));
        }

        public static Faker<BookResponse> GetBookResponse()
        {
            return new Faker<BookResponse>().CustomInstantiator(f => (
                new BookResponse(
                    Title: f.Random.String2(10),
                    Isbn: f.Random.String2(10),
                    Author: f.Person.FullName,
                    Gender: Gender.FromValue(f.Random.Int(0, 5)).Name,
                    Cover: Array.Empty<byte>()
                )
            ));
        }

        public static GetBookByIdResponse GetBookByIdResponse()
        {
            return new Faker<GetBookByIdResponse>().CustomInstantiator(f => (
                new GetBookByIdResponse(
                    Title: f.Random.String2(10),
                    Isbn: f.Random.String2(10),
                    Author: f.Person.FullName,
                    MeanScore: f.Random.Decimal(0, 5),
                    Gender: Gender.FromValue(f.Random.Int(0, 5)).Name,
                    BookData: GetBookDataResponse(),
                    Cover: Array.Empty<byte>(),
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
                    Description: description ?? f.Random.String2(20),
                    Cover: Array.Empty<byte>()
                )
            ));
        }
    }
}