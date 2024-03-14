using GoodReads.Application.Common.Pagination;

using MediatR;

namespace GoodReads.Application.Features.Ratings.GetPaginated
{
    public sealed record GetPaginatedRatingsRequest(
        int Page,
        int Size,
        Guid? BookId = null,
        Guid? UserId = null,
        int? OnlyScoresOf = null
    ) : PaginatedRequest(Page, Size),
        IRequest<PaginatedResponse<RatingResponse>>;
}