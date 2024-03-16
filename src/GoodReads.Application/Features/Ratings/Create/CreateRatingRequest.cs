using ErrorOr;

using MediatR;

namespace GoodReads.Application.Features.Ratings
{
    public sealed record CreateRatingRequest(
        int Score,
        string Description,
        CreateReadingRequest Reading,
        Guid UserId,
        Guid BookId
    ) : IRequest<ErrorOr<Guid>>;

    public sealed record CreateReadingRequest(
        DateTime InitiatedAt,
        DateTime FinishedAt
    );
}