using GoodReads.Domain.BookAggregate.Enums;
using GoodReads.Domain.BookAggregate.ValueObjects;
using GoodReads.Domain.Common;
using GoodReads.Domain.Common.Exceptions;

using Throw;

#pragma warning disable CS8618
namespace GoodReads.Domain.BookAggregate.Entities
{
    public sealed class Book : AggregateRoot
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Isbn { get; private set; }
        public string Author { get; private set; }
        public Gender Gender { get; private set; }
        public BookData BookData { get; private set; }
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

        public void SetBookData(BookData bookData)
        {
            BookData = bookData;
        }
    }
}