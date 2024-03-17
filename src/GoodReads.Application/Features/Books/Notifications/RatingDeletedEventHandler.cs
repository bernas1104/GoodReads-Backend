using GoodReads.Application.Common.Repositories.EntityFramework;
using GoodReads.Domain.BookAggregate.Entities;
using GoodReads.Domain.BookAggregate.ValueObjects;
using GoodReads.Domain.Common.Events;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GoodReads.Application.Features.Books.Notifications
{
    public sealed class RatingDeletedEventHandler :
        INotificationHandler<RatingDeleted>
    {
        private readonly IRepository<Book, BookId, Guid> _repository;
        private readonly ILogger<RatingDeletedEventHandler> _logger;

        public RatingDeletedEventHandler(
            IRepository<Book, BookId, Guid> repository,
            ILogger<RatingDeletedEventHandler> logger
        )
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Handle(
            RatingDeleted notification,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var book = await _repository.GetByIdAsync(
                BookId.Create(notification.BookId),
                cancellationToken
            );

            book!.RemoveRating(
                RatingId.Create(notification.RatingId),
                notification.Score
            );

            await _repository.UpdateAsync(book, cancellationToken);

            _logger.LogInformation(
                "Rating ({RatingId}) removed from Book ({BookId})",
                notification.RatingId,
                book.Id.Value
            );
        }

    }
}