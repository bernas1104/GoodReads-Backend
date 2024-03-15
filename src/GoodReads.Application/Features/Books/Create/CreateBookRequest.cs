using ErrorOr;

using MediatR;

namespace GoodReads.Application.Features.Books.Create
{
    public sealed record CreateBookRequest(
        string Title,
        string Description,
        string Isbn,
        string Author,
        int Gender,
        BookDataRequest BookData
        // byte[] Cover
    ) : IRequest<ErrorOr<Guid>>;

    public sealed record BookDataRequest(
        string Publisher,
        int YearOfPublication,
        int Pages
    );
}