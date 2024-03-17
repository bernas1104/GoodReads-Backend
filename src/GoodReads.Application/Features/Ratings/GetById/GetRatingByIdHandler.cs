using ErrorOr;

using GoodReads.Application.Common.Repositories.MongoDb;
using GoodReads.Application.Common.Services;
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
        private readonly ICachingService _cachingService;

        public GetRatingByIdHandler(
            IRepository<Rating, RatingId, Guid> repository,
            ILogger<GetRatingByIdHandler> logger,
            ICachingService cachingService
        )
        {
            _repository = repository;
            _logger = logger;
            _cachingService = cachingService;
        }

        public async Task<ErrorOr<RatingResponse>> Handle(
            GetRatingByIdRequest request,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var rating = await _cachingService.GetAsync<Rating>(
                request.Id.ToString(),
                cancellationToken
            );

            if (rating is not null)
            {
                return BuildResponse(rating);
            }

            rating = await _repository.GetByIdAsync(
                RatingId.Create(request.Id),
                cancellationToken
            );

            if (rating is null)
            {
                return RatingNotFoundError(request);
            }

            await _cachingService.SetAsync(
                rating.Id.Value.ToString(),
                rating,
                DateTimeOffset.UtcNow.AddMinutes(5),
                cancellationToken
            );

            return BuildResponse(rating);
        }

        private static RatingResponse BuildResponse(Rating rating) =>
            new RatingResponse(
                rating.Score.Value,
                rating.Description,
                new ReadingResponse(
                    rating.Reading.InitiatedAt,
                    rating.Reading.FinishedAt
                ),
                rating.CreatedAt
            );

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