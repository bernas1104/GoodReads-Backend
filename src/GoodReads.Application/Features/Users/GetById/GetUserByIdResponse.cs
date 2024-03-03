namespace GoodReads.Application.Features.Users.GetById
{
    public sealed record GetUserByIdResponse(
        string Name,
        string Email,
        int TotalRatings,
        DateTime CreatedAt
    ) : UserResponse(Name, TotalRatings);
}