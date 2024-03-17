using ErrorOr;

using GoodReads.Application.Common.Repositories.MongoDb;
using GoodReads.Domain.RatingAggregate.Entities;
using GoodReads.Domain.RatingAggregate.ValueObjects;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GoodReads.Application.Features.Ratings.GetById
{
    public sealed class GetRatingByIdHandler :
        IRequestHandler<GetRatingByIdRequest, ErrorOr<RatingResponse>>
    {
        private readonly IRepository<Rating, RatingId, Guid> _repository;
        private readonly ILogger<GetRatingByIdHandler> _logger;

        public GetRatingByIdHandler(
            IRepository<Rating, RatingId, Guid> repository,
            ILogger<GetRatingByIdHandler> logger
        )
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ErrorOr<RatingResponse>> Handle(
            GetRatingByIdRequest request,
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

            return new RatingResponse(
                rating.Score.Value,
                rating.Description,
                new ReadingResponse(
                    rating.Reading.InitiatedAt,
                    rating.Reading.FinishedAt
                ),
                rating.CreatedAt
            );
        }

        private Error RatingNotFoundError(GetRatingByIdRequest request)
        {
            _logger.LogError("Rating ({RatingId}) was not found", request.Id);

            return Error.NotFound(
                code: "Rating.NotFound",
                description: $"Rating ({request.Id}) was not found"
            );
        }
    }
}