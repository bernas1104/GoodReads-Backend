using ErrorOr;

using GoodReads.Domain.BookAggregate.Builders;
using GoodReads.Domain.BookAggregate.Entities;
using GoodReads.Domain.BookAggregate.Enums;
using GoodReads.Domain.BookAggregate.ValueObjects;

using GoodReads.Application.Common.Repositories.EntityFramework;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GoodReads.Application.Features.Books.Create
{
    public sealed class CreateBookHandler :
        IRequestHandler<CreateBookRequest, ErrorOr<Guid>>
    {
        private readonly IRepository<Book, BookId, Guid> _repository;
        private readonly ILogger<CreateBookHandler> _logger;

        public CreateBookHandler(
            IRepository<Book, BookId, Guid> repository,
            ILogger<CreateBookHandler> logger
        )
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ErrorOr<Guid>> Handle(
            CreateBookRequest request,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var book = await _repository.GetByFilterAsync(
                b => b.Isbn == request.Isbn,
                cancellationToken
            );

            if (book is not null)
            {
                return IsbnAlreadyUsedError(request);
            }

            book = BuildBook(request).GetBook();

            await _repository.AddAsync(book, cancellationToken);

            _logger.LogInformation(
                "Book {BookTitle} was registered successfully",
                book.Title
            );

            return book.Id.Value;
        }

        private Error IsbnAlreadyUsedError(CreateBookRequest request)
        {
            _logger.LogError("ISBN {ISBN} is already being used", request.Isbn);

            return Error.Conflict(
                code: "Book.Conflict",
                description: $"ISBN {request.Isbn} is already being used"
            );
        }

        private static BookBuilder BuildBook(CreateBookRequest request)
        {
            var builder = new BookBuilder(
                title: request.Title,
                isbn: request.Isbn,
                author: request.Author,
                gender: Gender.FromValue(request.Gender)
            );

            builder.AddDescription(request.Description);
            builder.AddCover(request.Cover);
            builder.AddBookData(
                request.BookData.Publisher,
                request.BookData.YearOfPublication,
                request.BookData.Pages
            );

            return builder;
        }
    }
}