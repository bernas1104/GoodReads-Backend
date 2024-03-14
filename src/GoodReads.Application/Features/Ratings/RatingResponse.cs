namespace GoodReads.Application.Features.Ratings
{
    public sealed record RatingResponse(
        decimal Score,
        string Description,
        ReadingResponse Reading,
        DateTime CreatedAt
    );
}