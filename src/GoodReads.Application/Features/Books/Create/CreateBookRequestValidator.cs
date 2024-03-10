using FluentValidation;

using GoodReads.Domain.BookAggregate.Enums;

namespace GoodReads.Application.Features.Books.Create
{
    public sealed class CreateBookRequestValidator : AbstractValidator<CreateBookRequest>
    {
        public CreateBookRequestValidator()
        {
            RuleFor(x => x.Title).NotEmpty();

            RuleFor(x => x.Description).NotEmpty();

            RuleFor(x => x.Isbn).NotEmpty();

            RuleFor(x => x.Author).NotEmpty();

            RuleFor(x => x.Gender).Must(x => Gender.TryFromValue(x, out _));

            RuleFor(x => x.BookData).SetValidator(new BookDataRequestValidator());
        }
    }

    public sealed class BookDataRequestValidator : AbstractValidator<BookDataRequest>
    {
        public BookDataRequestValidator()
        {
            RuleFor(x => x.Publisher).NotEmpty();

            RuleFor(x => x.YearOfPublication)
                .GreaterThanOrEqualTo(1900)
                .LessThanOrEqualTo(DateTime.UtcNow.Year);

            RuleFor(x => x.Pages).GreaterThan(0);
        }
    }
}