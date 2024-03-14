using ErrorOr;

using MediatR;

namespace GoodReads.Application.Features.Ratings.Update
{
    public sealed record UpdateRatingRequest(
        Guid Id,
        string Description
    ) : IRequest<ErrorOr<Updated>>;
}