namespace GoodReads.Application.Features.Books.GetById
{
    public record GetBookByIdResponse(
        string Title,
        string Description,
        string Isbn,
        string Author,
        decimal MeanScore,
        string Gender,
        BookDataResponse BookData,
        // byte[] Cover,
        IEnumerable<Guid> RatingIds

    ) : BookResponse(
        Title,
        Isbn,
        Author,
        Gender
        // Cover
    );

    public record BookDataResponse(
        string Publisher,
        int YearOfPublication,
        int Pages
    );
}