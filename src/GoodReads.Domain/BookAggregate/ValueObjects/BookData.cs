using GoodReads.Domain.Common;

namespace GoodReads.Domain.BookAggregate.ValueObjects
{
    public sealed class BookData : ValueObject
    {
        public string Publisher { get; private set; }
        public int YearOfPublication { get; private set; }
        public int Pages { get; private set; }

        public BookData(
            string publisher,
            int yearOfPublication,
            int pages
        )
        {
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