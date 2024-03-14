using System.Diagnostics.CodeAnalysis;

using FluentValidation;

namespace GoodReads.Application.Features.Users.GetById
{
    [ExcludeFromCodeCoverage]
    public sealed class GetUserByIdRequestValidator : AbstractValidator<GetUserByIdRequest>
    {
        public GetUserByIdRequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}