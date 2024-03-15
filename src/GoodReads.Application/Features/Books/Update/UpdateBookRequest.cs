using ErrorOr;

using MediatR;

namespace GoodReads.Application.Features.Books.Update
{
    public sealed record UpdateBookRequest(
        Guid Id,
        string Description
        // byte[] Cover
    ) : IRequest<ErrorOr<Updated>>;
}