using FluentValidation;

using GoodReads.Application.Common.Pagination;

namespace GoodReads.Application.Features.Users.GetPaginated
{
    public sealed class GetPaginatedUsersRequestValidator : AbstractValidator<GetPaginatedUsersRequest>
    {
        public GetPaginatedUsersRequestValidator()
        {
            RuleFor(x => x).SetValidator(new PaginatedRequestValidator());
        }
    }
}