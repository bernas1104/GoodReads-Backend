using System.Diagnostics.CodeAnalysis;

using FluentValidation;

namespace GoodReads.Application.Features.Ratings.Update
{
    [ExcludeFromCodeCoverage]
    public sealed class UpdateRatingRequestValidator :
        AbstractValidator<UpdateRatingRequest>
    {
        public UpdateRatingRequestValidator()
        {
            RuleFor(r => r.Id).NotEmpty();

            RuleFor(r => r.Description).NotEmpty();
        }
    }
}