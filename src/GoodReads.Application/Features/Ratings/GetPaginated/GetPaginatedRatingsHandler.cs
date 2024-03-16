using System.Linq.Expressions;

using GoodReads.Application.Common.Pagination;
using GoodReads.Application.Common.Repositories.MongoDb;
using GoodReads.Domain.RatingAggregate.Entities;
using GoodReads.Domain.RatingAggregate.ValueObjects;

using LinqKit;

using MediatR;

namespace GoodReads.Application.Features.Ratings.GetPaginated
{
    public sealed class GetPaginatedRatingsHandler :
        IRequestHandler<GetPaginatedRatingsRequest, PaginatedResponse<RatingResponse>>
    {
        private readonly IRepository<Rating, RatingId, Guid> _repository;

        public GetPaginatedRatingsHandler(IRepository<Rating, RatingId, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResponse<RatingResponse>> Handle(
            GetPaginatedRatingsRequest request,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var filter = EmptyFilter;
            filter = request.UserId == null ?
                filter.And(r => r.BookId.Equals(BookId.Create(request.BookId!.Value))) :
                filter.And(r => r.UserId.Equals(UserId.Create(request.UserId!.Value)));

            filter = request.OnlyScoresOf == null ?
                filter.And(EmptyFilter) :
                filter.And(r => r.Score.Equals(new Score(request.OnlyScoresOf.Value)));

            var ratings = await _repository.GetPaginatedAsync(
                filter,
                request.Page,
                request.Size,
                cancellationToken
            );

            var ratingsCount = await _repository.GetCountAsync(filter, cancellationToken);

            return new PaginatedResponse<RatingResponse>(
                Data: ratings.Select(
                    r => new RatingResponse(
                        r.Score.Value,
                        r.Description,
                        new ReadingResponse(
                            r.Reading.InitiatedAt,
                            r.Reading.FinishedAt
                        ),
                        r.CreatedAt
                    )
                ),
                CurrentPage: request.Page,
                TotalItens: (int)ratingsCount,
                TotalPages: (int)Math.Ceiling(ratingsCount / (decimal)request.Size),
                PageSize: request.Size
            );
        }

        private static Expression<Func<Rating, bool>> EmptyFilter => r => true;
    }
}