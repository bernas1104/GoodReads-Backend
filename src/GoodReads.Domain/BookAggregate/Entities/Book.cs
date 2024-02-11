using GoodReads.Domain.BookAggregate.Enums;
using GoodReads.Domain.BookAggregate.ValueObjects;
using GoodReads.Domain.Common;

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
        public decimal MeanScore { get; private set; }
        public Gender Gender { get; private set; }
        public BookData BookData { get; private set; }
        public IEnumerable<byte> Cover { get; private set; }
        public IReadOnlyList<Guid> Ratings { get => _ratings.ToList(); }
        private readonly List<Guid> _ratings;

        public Book(string title, string isbn, string author, Gender gender)
        {
            Title = title;
            Isbn = isbn;
            Author = author;
            Gender = gender;
            MeanScore = default;

            Cover = new List<byte>();
            _ratings = new List<Guid>();
        }

        public void SetCover(IEnumerable<byte> cover) => Cover = cover;

        public void SetDescription(string description)
        {
            Description = description;
        }

        public void SetBookData(BookData bookData)
        {
            BookData = bookData;
        }
    }
}