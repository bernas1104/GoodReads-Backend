using FluentValidation;

namespace GoodReads.Application.Common.Pagination
{
    public sealed class PaginatedRequestValidator : AbstractValidator<PaginatedRequest>
    {
        public PaginatedRequestValidator()
        {
            RuleFor(x => x.Page).GreaterThan(0);
            RuleFor(x => x.Size).GreaterThan(0);
        }
    }
}