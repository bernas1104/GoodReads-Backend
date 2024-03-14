using System.Diagnostics.CodeAnalysis;

using FluentValidation;

namespace GoodReads.Application.Features.Ratings.Create
{
    [ExcludeFromCodeCoverage]
    public sealed class CreateReadingRequestValidator :
        AbstractValidator<CreateReadingRequest>
    {
        public CreateReadingRequestValidator()
        {
            RuleFor(r => r.InitiatedAt).NotEmpty();

            RuleFor(r => r.FinishedAt)
                .NotEmpty()
                .GreaterThan(r => r.InitiatedAt);
        }
    }
}