using ErrorOr;

using MediatR;

namespace GoodReads.Application.Features.Books.Delete
{
    public record DeleteBookRequest(Guid Id) : IRequest<ErrorOr<Deleted>>;
}