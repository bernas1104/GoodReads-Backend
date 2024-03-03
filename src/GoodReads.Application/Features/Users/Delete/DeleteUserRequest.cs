using ErrorOr;

using MediatR;

namespace GoodReads.Application.Features.Users.Delete
{
    public record DeleteUserRequest(Guid Id) : IRequest<ErrorOr<Deleted>>;
}