using ErrorOr;

using MediatR;

namespace GoodReads.Application.Features.Users.GetById
{
    public sealed record GetUserByIdRequest(Guid Id) :
        IRequest<ErrorOr<GetUserByIdResponse>>;
}