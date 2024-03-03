using ErrorOr;

using MediatR;

namespace GoodReads.Application.Features.Users.Create
{
    public sealed record CreateUserRequest(string Name, string Email) :
        UserRequest(Name, Email),
        IRequest<ErrorOr<Guid>>;
}