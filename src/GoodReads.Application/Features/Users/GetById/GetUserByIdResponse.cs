namespace GoodReads.Application.Features.Users.GetById
{
    public sealed record GetUserByIdResponse(
        string Name,
        string Email,
        int TotalRatings,
        UserRatingIdsResponse Ratings,
        DateTime CreatedAt
    ) : UserResponse(Name, TotalRatings);

    public sealed record UserRatingIdsResponse(IEnumerable<Guid> RatingIds);
}