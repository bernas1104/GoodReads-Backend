using ErrorOr;

using GoodReads.Domain.BookAggregate.Entities;
using GoodReads.Domain.BookAggregate.ValueObjects;

using GoodReads.Domain.Common.Interfaces.Repositories.EntityFramework;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GoodReads.Application.Features.Books.Delete
{
    public sealed class DeleteBookHandler :
        IRequestHandler<DeleteBookRequest, ErrorOr<Deleted>>
    {
        private readonly IRepository<Book, BookId, Guid> _repository;
        private readonly ILogger<DeleteBookHandler> _logger;

        public DeleteBookHandler(
            IRepository<Book, BookId, Guid> repository,
            ILogger<DeleteBookHandler> logger
        )
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ErrorOr<Deleted>> Handle(
            DeleteBookRequest request,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var book = await _repository.GetByIdAsync(
                BookId.Create(request.Id),
                cancellationToken
            );

            if (book is null)
            {
                return BookNotFoundError(request);
            }

            book.Delete();

            await _repository.UpdateAsync(book, cancellationToken);

            _logger.LogInformation("Book ({BookId}) was deleted", book.Id.Value);

            return Result.Deleted;
        }

        private Error BookNotFoundError(DeleteBookRequest request)
        {
            _logger.LogWarning("Book ({BookId}) was not found", request.Id);

            return Error.NotFound(
                code: "Book.NotFound",
                description: $"Book ({request.Id}) was not found"
            );
        }
    }
}