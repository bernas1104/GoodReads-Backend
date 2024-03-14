using ErrorOr;

using MediatR;

namespace GoodReads.Application.Features.Ratings.Delete
{
    public sealed record DeleteRatingRequest(Guid Id) : IRequest<ErrorOr<Deleted>>;
}