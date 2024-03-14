using System.Diagnostics.CodeAnalysis;

using FluentValidation;

namespace GoodReads.Application.Features.Books.GetById
{
    [ExcludeFromCodeCoverage]
    public sealed class GetBookByIdRequestValidator :
        AbstractValidator<GetBookByIdRequest>
    {
        public GetBookByIdRequestValidator()
        {
            RuleFor(b => b.Id).NotEmpty();
        }
    }
}