using System.Diagnostics.CodeAnalysis;

using FluentValidation;

namespace GoodReads.Application.Features.Users.Create
{
    [ExcludeFromCodeCoverage]
    public sealed class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x).SetValidator(new UserRequestValidator());
        }
    }
}