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
        BookDataRequest BookData,
        byte[] Cover
    ) : BookRequest(
        Title,
        Description,
        Isbn,
        Author,
        Gender,
        BookData,
        Cover
    ), IRequest<ErrorOr<Guid>>;
}