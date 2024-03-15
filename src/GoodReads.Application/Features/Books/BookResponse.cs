namespace GoodReads.Application.Features.Books
{
    public record BookResponse(
        string Title,
        string Isbn,
        string Author,
        string Gender
        // byte[] Cover
    );
}