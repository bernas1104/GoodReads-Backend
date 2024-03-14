using System.Diagnostics.CodeAnalysis;

using FluentValidation;

using GoodReads.Application.Common.Pagination;

namespace GoodReads.Application.Features.Ratings.GetPaginated
{
    [ExcludeFromCodeCoverage]
    public sealed class GetPaginatedRatingsRequestValidator :
        AbstractValidator<GetPaginatedRatingsRequest>
    {
        public GetPaginatedRatingsRequestValidator()
        {
            RuleFor(x => x).SetValidator(new PaginatedRequestValidator());

            RuleFor(x => x.BookId)
                .NotEmpty()
                .Unless(x => x.UserId != null);

            RuleFor(x => x.UserId)
                .NotEmpty()
                .Unless(x => x.BookId != null);

            RuleFor(x => x.OnlyScoresOf)
                .InclusiveBetween(1, 5)
                .When(x => x.OnlyScoresOf != null);
        }
    }
}