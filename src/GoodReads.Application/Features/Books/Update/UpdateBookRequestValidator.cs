using System.Diagnostics.CodeAnalysis;

using FluentValidation;

namespace GoodReads.Application.Features.Books.Update
{
    [ExcludeFromCodeCoverage]
    public sealed class UpdateBookRequestValidator :
        AbstractValidator<UpdateBookRequest>
    {
        public UpdateBookRequestValidator()
        {
            RuleFor(b => b.Id).NotEmpty();

            RuleFor(b => b.Description).NotEmpty();
        }
    }
}