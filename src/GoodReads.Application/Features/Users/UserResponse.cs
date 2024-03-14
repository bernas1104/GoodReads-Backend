namespace GoodReads.Application.Features.Users
{
    public record UserResponse(
        string Name,
        int TotalRatings
    );
}