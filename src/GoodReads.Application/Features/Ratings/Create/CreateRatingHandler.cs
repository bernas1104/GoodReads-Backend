using ErrorOr;

using GoodReads.Application.Common.Repositories.MongoDb;
using GoodReads.Domain.Common.Events;
using GoodReads.Domain.RatingAggregate.Entities;
using GoodReads.Domain.RatingAggregate.ValueObjects;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GoodReads.Application.Features.Ratings.Create
{
    public sealed class CreateRatingHandler :
        IRequestHandler<CreateRatingRequest, ErrorOr<Guid>>
    {
        private readonly IRepository<Rating, RatingId, Guid> _repository;
        private readonly IPublisher _publisher;
        private readonly ILogger<CreateRatingHandler> _logger;

        public CreateRatingHandler(
            IRepository<Rating, RatingId, Guid> repository,
            IPublisher publisher,
            ILogger<CreateRatingHandler> logger
        )
        {
            _repository = repository;
            _publisher = publisher;
            _logger = logger;
        }

        public async Task<ErrorOr<Guid>> Handle(
            CreateRatingRequest request,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var rating = await _repository.GetByFilterAsync(
                r => r.UserId.Equals(UserId.Create(request.UserId)) &&
                    r.BookId.Equals(BookId.Create(request.BookId)) &&
                    r.DeletedAt == null,
                cancellationToken
            );

            if (rating is not null)
            {
                return UserAlreadyPostedRatingForBookError(request);
            }

            rating = CreateRating(request);

            await _repository.AddAsync(rating, cancellationToken);

            await _publisher.Publish(
                RatingCreated.Create(rating),
                cancellationToken
            );

            _logger.LogInformation(
                "User ({UserId}) rating for book ({BookId}) was created successfully",
                rating.UserId.Value,
                rating.BookId.Value
            );

            return rating.Id.Value;
        }

        private Error UserAlreadyPostedRatingForBookError(CreateRatingRequest request)
        {
            _logger.LogError(
                "User ({UserId}) already posted a rating for book ({BookId})",
                request.UserId,
                request.BookId
            );

            return Error.Conflict(
                code: "Rating.Conflit",
                description: $"User ({request.UserId}) already posted a rating " +
                    $"for book ({request.BookId})"
            );
        }

        private Rating CreateRating(CreateRatingRequest request) => Rating.Create(
            score: new Score(request.Score),
            description: request.Description,
            reading: new Reading(
                request.Reading.InitiatedAt,
                request.Reading.FinishedAt
            ),
            userId: request.UserId,
            bookId: request.BookId
        );
    }
}