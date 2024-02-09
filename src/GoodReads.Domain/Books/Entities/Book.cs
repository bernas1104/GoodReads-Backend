using GoodReads.Domain.Books.Enums;
using GoodReads.Domain.Common;
using GoodReads.Domain.Common.Exceptions;
using GoodReads.Domain.Common.Interfaces.Providers;

using Throw;

#pragma warning disable CS8618
namespace GoodReads.Domain.Books.Entities
{
    public sealed class Book : AggregateRoot
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Isbn { get; private set; }
        public string Author { get; private set; }
        public string Publisher { get; private set; }
        public Gender Gender { get; private set; }
        public int YearOfPublication { get; private set; }
        public int Pages { get; private set; }
        public IEnumerable<byte> Cover { get; private set; }
        public IEnumerable<Rating> Ratings { get; private set; }

        public Book(string title, string isbn, string author, Gender gender)
        {
            Title = title;
            Isbn = isbn;
            Author = author;
            Gender = gender;
            Cover = new List<byte>();
        }

        public decimal MeanScore { 
            get {
                if (!Ratings.Any())
                {
                    return 0;
                }

                return Ratings.Sum(x => x.Score.Value) / Ratings.Count();
            } 
        }

        public void SetCover(IEnumerable<byte> cover) => Cover = cover;

        public void SetDescription(string description)
        {
            description.Throw(() => new DomainException("'Description' is required"))
                .IfEmpty()
                .IfWhiteSpace();

            Description = description;
        }

        public void SetPages(int pages)
        {
            pages.Throw(() => new DomainException("'Pages' must be greater than 0"))
                .IfLessThan(1);

            Pages = pages;
        }

        public void SetPublisher(string publisher)
        {
            publisher.Throw(() => new DomainException("'Publisher' is required"))
                .IfEmpty()
                .IfWhiteSpace();

            Publisher = publisher;
        }

        public void SetYearOfPublication(int yearOfPublication, IDateProvider dateProvider) 
        {
            var currentYear = dateProvider.GetCurrentYear();

            yearOfPublication
                .Throw(() => new DomainException($"'YearOfPublication' must be between 1900 and {currentYear}"))
                .IfLessThan(1900)
                .IfGreaterThan(currentYear);

            YearOfPublication = yearOfPublication;
        }
    }
}