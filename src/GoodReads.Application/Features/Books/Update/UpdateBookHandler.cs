using ErrorOr;

using GoodReads.Domain.BookAggregate.Entities;
using GoodReads.Domain.BookAggregate.ValueObjects;

using GoodReads.Application.Common.Repositories.EntityFramework;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GoodReads.Application.Features.Books.Update
{
    public sealed class UpdateBookHandler :
        IRequestHandler<UpdateBookRequest, ErrorOr<Updated>>
    {
        private readonly IRepository<Book, BookId, Guid> _repository;
        private readonly ILogger<UpdateBookHandler> _logger;

        public UpdateBookHandler(
            IRepository<Book, BookId, Guid> repository,
            ILogger<UpdateBookHandler> logger
        )
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ErrorOr<Updated>> Handle(
            UpdateBookRequest request,
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

            book.Update(request.Description, request.Cover);

            await _repository.UpdateAsync(book, cancellationToken);

            _logger.LogInformation(
                "Book ({BookId}) was updated successfully",
                book.Id.Value
            );

            return Result.Updated;
        }

        private Error BookNotFoundError(UpdateBookRequest request)
        {
            _logger.LogError("Book ({BookId}) was not found", request.Id);

            return Error.NotFound(
                code: "Book.NotFound",
                description: $"Book ({request.Id}) was not found"
            );
        }
    }
}