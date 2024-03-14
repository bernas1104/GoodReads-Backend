using ErrorOr;

using MediatR;

namespace GoodReads.Application.Features.Ratings.GetById
{
    public sealed record GetRatingByIdRequest(Guid Id) :
        IRequest<ErrorOr<RatingResponse>>;
}