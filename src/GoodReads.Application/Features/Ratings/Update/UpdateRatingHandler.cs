using ErrorOr;

using GoodReads.Application.Common.Repositories.MongoDb;
using GoodReads.Domain.RatingAggregate.Entities;
using GoodReads.Domain.RatingAggregate.ValueObjects;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GoodReads.Application.Features.Ratings.Update
{
    public sealed class UpdateRatingHandler :
        IRequestHandler<UpdateRatingRequest, ErrorOr<Updated>>
    {
        private readonly IRepository<Rating, RatingId, Guid> _repository;
        private readonly ILogger<UpdateRatingHandler> _logger;

        public UpdateRatingHandler(
            IRepository<Rating, RatingId, Guid> repository,
            ILogger<UpdateRatingHandler> logger
        )
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ErrorOr<Updated>> Handle(
            UpdateRatingRequest request,
            CancellationToken cancellationToken
        )
        {
            var rating = await _repository.GetByIdAsync(
                RatingId.Create(request.Id),
                cancellationToken
            );

            if (rating is null)
            {
                return RatingNotFoundError(request);
            }

            rating.Update(request.Description);

            await _repository.UpdateAsync(
                r => r.Id.Equals(RatingId.Create(request.Id)),
                rating,
                cancellationToken
            );

            _logger.LogInformation(
                "Rating ({RatingId}) was updated successfully",
                rating.Id.Value
            );

            return Result.Updated;
        }

        private Error RatingNotFoundError(UpdateRatingRequest request)
        {
            _logger.LogError("Rating ({RatingId}) was not found", request.Id);

            return Error.NotFound(
                code: "Rating.NotFound",
                description: $"Rating ({request.Id}) was not found"
            );
        }
    }
}