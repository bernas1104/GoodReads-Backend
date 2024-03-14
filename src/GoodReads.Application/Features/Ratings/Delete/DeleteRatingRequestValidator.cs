using System.Diagnostics.CodeAnalysis;

using FluentValidation;

namespace GoodReads.Application.Features.Ratings.Delete
{
    [ExcludeFromCodeCoverage]
    public sealed class DeleteRatingRequestValidator :
        AbstractValidator<DeleteRatingRequest>
    {
        public DeleteRatingRequestValidator()
        {
            RuleFor(r => r.Id).NotEmpty();
        }
    }
}