using System.Diagnostics.CodeAnalysis;

using FluentValidation;

namespace GoodReads.Application.Features.Books.Delete
{
    [ExcludeFromCodeCoverage]
    public sealed class DeleteBookRequestValidator :
        AbstractValidator<DeleteBookRequest>
    {
        public DeleteBookRequestValidator()
        {
            RuleFor(b => b.Id).NotEmpty();
        }
    }
}