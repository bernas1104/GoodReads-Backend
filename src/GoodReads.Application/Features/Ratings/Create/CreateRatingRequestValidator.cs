using System.Diagnostics.CodeAnalysis;

using FluentValidation;

namespace GoodReads.Application.Features.Ratings.Create
{
    [ExcludeFromCodeCoverage]
    public sealed class CreateRatingRequestValidator :
        AbstractValidator<CreateRatingRequest>
    {
        public CreateRatingRequestValidator()
        {
            RuleFor(r => r.Score).NotEmpty();

            RuleFor(r => r.Description).NotEmpty();

            RuleFor(r => r.Reading).SetValidator(new CreateReadingRequestValidator());

            RuleFor(r => r.UserId).NotEmpty();

            RuleFor(r => r.BookId).NotEmpty();
        }
    }
}