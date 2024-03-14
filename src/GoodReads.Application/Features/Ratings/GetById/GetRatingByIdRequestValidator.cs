using System.Diagnostics.CodeAnalysis;

using FluentValidation;

namespace GoodReads.Application.Features.Ratings.GetById
{
    [ExcludeFromCodeCoverage]
    public sealed class GetRatingByIdRequestValidator :
        AbstractValidator<GetRatingByIdRequest>
    {
        public GetRatingByIdRequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}