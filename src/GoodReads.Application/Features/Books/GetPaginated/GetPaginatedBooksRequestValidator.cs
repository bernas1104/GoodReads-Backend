using System.Diagnostics.CodeAnalysis;

using FluentValidation;

using GoodReads.Application.Common.Pagination;

namespace GoodReads.Application.Features.Books.GetPaginated
{
    [ExcludeFromCodeCoverage]
    public sealed class GetPaginatedBooksRequestValidator :
        AbstractValidator<GetPaginatedBooksRequest>
    {
        public GetPaginatedBooksRequestValidator()
        {
            RuleFor(b => b).SetValidator(new PaginatedRequestValidator());
        }
    }
}