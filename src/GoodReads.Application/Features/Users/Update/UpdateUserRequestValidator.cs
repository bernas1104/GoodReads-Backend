using System.Diagnostics.CodeAnalysis;

using FluentValidation;

namespace GoodReads.Application.Features.Users.Update
{
    public sealed class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        [ExcludeFromCodeCoverage]
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty();

            RuleFor(x => x).SetValidator(new UserRequestValidator());
        }
    }
}