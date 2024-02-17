using ErrorOr;

using MediatR;

namespace GoodReads.Application.Features.Books.Register
{
    /// <summary>
    /// Register Book request
    /// </summary>
    /// <param name="Title">Book title, required</param>
    /// <param name="Description">Book description, required</param>
    /// <param name="Isbn">Book ISBN, unique</param>
    /// <param name="Author">Book author, required</param>
    /// <param name="Gender">Book gender, required</param>
    /// <param name="BookData">Book aditional data</param>
    public record RegisterBookRequest(
        string Title,
        string Description,
        string Isbn,
        string Author,
        int Gender,
        RegisterBookDataRequest BookData
    ) : IRequest<ErrorOr<Success>>;

    /// <summary>
    /// Register Book additional data
    /// </summary>
    /// <param name="Publisher">Book publisher, required</param>
    /// <param name="YearOfPublication">Book year of publication, required</param>
    /// <param name="Pages">Book number of pages, required</param>
    public record RegisterBookDataRequest(
        string Publisher,
        int YearOfPublication,
        int Pages
    );
}