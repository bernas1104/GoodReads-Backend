namespace GoodReads.Application.Features.Ratings
{
    public sealed record RatingResponse(
        int Score,
        string Description,
        ReadingResponse Reading,
        DateTime CreatedAt
    );
}