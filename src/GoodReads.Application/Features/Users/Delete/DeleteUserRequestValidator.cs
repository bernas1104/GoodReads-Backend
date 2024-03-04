using FluentValidation;

namespace GoodReads.Application.Features.Users.Delete
{
    public sealed class DeleteUserRequestValidator : AbstractValidator<DeleteUserRequest>
    {
        public DeleteUserRequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}