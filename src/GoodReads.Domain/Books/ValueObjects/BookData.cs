using GoodReads.Domain.Common;
using GoodReads.Domain.Common.Exceptions;
using GoodReads.Domain.Common.Interfaces.Providers;

using Throw;

namespace GoodReads.Domain.Books.ValueObjects
{
    public class BookData : ValueObject
    {
        public string Publisher { get; private set; }
        public int YearOfPublication { get; private set; }
        public int Pages { get; private set; }

        public BookData(
            string publisher,
            int yearOfPublication,
            int pages,
            IDateProvider dateProvider
        )
        {
            var currentYear = dateProvider.GetCurrentYear();

            publisher.Throw(() => new DomainException("'Publisher' is required"))
                .IfEmpty()
                .IfWhiteSpace();

            yearOfPublication
                .Throw(() => new DomainException($"'YearOfPublication' is required"))
                .IfDefault()
                .Throw(() => new DomainException("'YearOfPublication' must be equal to or less than current year"))
                .IfGreaterThan(currentYear);

            pages.Throw(() => new DomainException("'Pages' must be greater than 0"))
                .IfLessThan(1);

            Publisher = publisher;
            YearOfPublication = yearOfPublication;
            Pages = pages;
        }

        public override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Publisher;
            yield return YearOfPublication;
            yield return Pages;
        }
    }
}