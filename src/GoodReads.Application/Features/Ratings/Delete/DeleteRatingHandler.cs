using ErrorOr;

using GoodReads.Application.Common.Repositories.MongoDb;
using GoodReads.Domain.Common.Events;
using GoodReads.Domain.RatingAggregate.Entities;
using GoodReads.Domain.RatingAggregate.ValueObjects;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GoodReads.Application.Features.Ratings.Delete
{
    public sealed class DeleteRatingHandler :
        IRequestHandler<DeleteRatingRequest, ErrorOr<Deleted>>
    {
        private readonly IRepository<Rating, RatingId, Guid> _repository;
        private readonly IPublisher _publisher;
        private readonly ILogger<DeleteRatingHandler> _logger;

        public DeleteRatingHandler(
            IRepository<Rating, RatingId, Guid> repository,
            IPublisher publisher,
            ILogger<DeleteRatingHandler> logger
        )
        {
            _repository = repository;
            _publisher = publisher;
            _logger = logger;
        }

        public async Task<ErrorOr<Deleted>> Handle(
            DeleteRatingRequest request,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var rating = await _repository.GetByIdAsync(
                RatingId.Create(request.Id),
                cancellationToken
            );

            if (rating is null)
            {
                return RatingNotFoundError(request);
            }

            await _repository.DeleteAsync(
                RatingId.Create(request.Id),
                cancellationToken
            );

            await _publisher.Publish(
                RatingDeleted.Create(rating),
                cancellationToken
            );

            _logger.LogInformation(
                "Rating ({RatingId}) was deleted successfully",
                request.Id
            );

            return Result.Deleted;
        }

        private Error RatingNotFoundError(DeleteRatingRequest request)
        {
            _logger.LogWarning("Rating ({RatingId}) was not found", request.Id);

            return Error.NotFound(
                code: "Rating.NotFound",
                description: $"Rating ({request.Id}) was not found"
            );
        }
    }
}