namespace GoodReads.Application.Features.Books
{
    public abstract record BookRequest(
        string Title,
        string Description,
        string Isbn,
        string Author,
        int Gender,
        BookDataRequest BookData,
        byte[] Cover
    );

    public sealed record BookDataRequest(
        string Publisher,
        int YearOfPublication,
        int Pages
    );
}