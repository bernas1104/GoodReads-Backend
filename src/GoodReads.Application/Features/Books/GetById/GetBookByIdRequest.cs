using ErrorOr;

using MediatR;

namespace GoodReads.Application.Features.Books.GetById
{
    public record GetBookByIdRequest(
        Guid Id
    ) : IRequest<ErrorOr<GetBookByIdResponse>>;
}