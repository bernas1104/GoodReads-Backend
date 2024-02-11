using GoodReads.Domain.BookAggregate.Entities;
using GoodReads.Domain.BookAggregate.Enums;
using GoodReads.Domain.BookAggregate.Interfaces.Builders;
using GoodReads.Domain.BookAggregate.ValueObjects;

#pragma warning disable CS8618
namespace GoodReads.Domain.BookAggregate.Builders
{
    public class BookBuilder : IBookBuilder
    {
        private Book _book;

        public BookBuilder(
            string title,
            string isbn,
            string author,
            Gender gender
        )
        {
            Reset(title, isbn, author, gender);
        }

        private void Reset(
            string title,
            string isbn,
            string author,
            Gender gender
        )
        {
            _book = new Book(title, isbn, author, gender);
        }

        public IBookBuilder AddCover(IEnumerable<byte> cover)
        {
            _book.SetCover(cover);
            return this;
        }

        public IBookBuilder AddDescription(string description)
        {
            _book.SetDescription(description);
            return this;
        }

        public IBookBuilder AddBookData(
            string publisher,
            int yearOfPublication,
            int pages
        )
        {
            _book.SetBookData(
                new BookData(
                    publisher,
                    yearOfPublication,
                    pages
                )
            );
            return this;
        }

        public Book GetBook() => _book;
    }
}