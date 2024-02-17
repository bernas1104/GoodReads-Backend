using GoodReads.Domain.BookAggregate.Enums;
using GoodReads.Domain.BookAggregate.ValueObjects;
using GoodReads.Domain.Common;

#pragma warning disable CS8618
namespace GoodReads.Domain.BookAggregate.Entities
{
    public sealed class Book : AggregateRoot<BookId, Guid>
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Isbn { get; private set; }
        public string Author { get; private set; }
        public MeanScore MeanScore { get; private set; }
        public Gender Gender { get; private set; }
        public BookData BookData { get; private set; }
        public IEnumerable<byte> Cover { get; private set; }
        public IReadOnlyList<Guid> Ratings { get => _ratings.AsReadOnly(); }
        private readonly List<Guid> _ratings;

        private Book(string title, string isbn, string author, Gender gender)
            : base (BookId.CreateUnique())
        {
            Title = title;
            Isbn = isbn;
            Author = author;
            Gender = gender;
            MeanScore = new ();

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

        public void AddRating(Guid ratingId, int ratingScore)
        {
            _ratings.Add(ratingId);

            MeanScore.Update(ratingScore);

            Update();
        }

        public static Book Create(
            string title,
            string isbn,
            string author,
            Gender gender
        )
        {
            return new Book(title, isbn, author, gender);
        }
    }
}