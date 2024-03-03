using ErrorOr;

using MediatR;

namespace GoodReads.Application.Features.Users.Update
{
    public sealed record UpdateUserRequest(Guid Id, string Name, string Email) :
        UserRequest(Name, Email),
        IRequest<ErrorOr<Updated>>;
}