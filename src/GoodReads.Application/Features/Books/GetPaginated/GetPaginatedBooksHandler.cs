using GoodReads.Domain.BookAggregate.Entities;
using GoodReads.Domain.BookAggregate.ValueObjects;
using GoodReads.Domain.Common.Interfaces.Repositories.EntityFramework;

using MediatR;

namespace GoodReads.Application.Features.Books.GetPaginated
{
    public sealed class GetPaginatedBooksHandler :
        IRequestHandler<GetPaginatedBooksRequest, GetPaginatedBooksResponse>
    {
        private readonly IRepository<Book, BookId, Guid> _repository;

        public GetPaginatedBooksHandler(IRepository<Book, BookId, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<GetPaginatedBooksResponse> Handle(
            GetPaginatedBooksRequest request,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var books = await _repository.GetPaginatedAsync(
                request.Page,
                request.Size,
                cancellationToken
            );

            var booksCount = await _repository.GetCountAsync(cancellationToken);

            return new GetPaginatedBooksResponse(
                Data: books.Select(b => new BookResponse(
                    Title: b.Title,
                    Isbn: b.Isbn,
                    Author: b.Author,
                    Gender: b.Gender.Name,
                    Cover: b.Cover.ToArray()
                )),
                CurrentPage: request.Page,
                TotalItens: booksCount,
                TotalPages: (int)Math.Ceiling(booksCount / (decimal) request.Size),
                PageSize: request.Size
            );
        }

    }
}