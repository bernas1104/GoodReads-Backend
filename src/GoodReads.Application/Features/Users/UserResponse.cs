namespace GoodReads.Application.Features.Users
{
    public abstract record UserResponse(
        string Name,
        int TotalRatings
    );
}