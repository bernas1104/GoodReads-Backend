using GoodReads.Domain.BookAggregate.Entities;
using GoodReads.Domain.BookAggregate.ValueObjects;
using GoodReads.Application.Common.Repositories.EntityFramework;

using MediatR;
using GoodReads.Application.Common.Pagination;

namespace GoodReads.Application.Features.Books.GetPaginated
{
    public sealed class GetPaginatedBooksHandler :
        IRequestHandler<GetPaginatedBooksRequest, PaginatedResponse<BookResponse>>
    {
        private readonly IRepository<Book, BookId, Guid> _repository;

        public GetPaginatedBooksHandler(IRepository<Book, BookId, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResponse<BookResponse>> Handle(
            GetPaginatedBooksRequest request,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var books = await _repository.GetPaginatedAsync(
                null,
                request.Page,
                request.Size,
                cancellationToken
            );

            var booksCount = await _repository.GetCountAsync(cancellationToken);

            return new PaginatedResponse<BookResponse>(
                Data: books.Select(b => new BookResponse(
                    Title: b.Title,
                    Isbn: b.Isbn,
                    Author: b.Author,
                    Gender: b.Gender.Name
                )),
                CurrentPage: request.Page,
                TotalItens: booksCount,
                TotalPages: (int)Math.Ceiling(booksCount / (decimal) request.Size),
                PageSize: request.Size
            );
        }

    }
}