using GoodReads.Domain.Books.Entities;
using GoodReads.Domain.Books.Enums;
using GoodReads.Domain.Books.Interfaces.Builders;
using GoodReads.Domain.Books.ValueObjects;
using GoodReads.Domain.Common.Interfaces.Providers;

#pragma warning disable CS8618
namespace GoodReads.Domain.Books.Builders
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
            int pages,
            IDateProvider dateProvider
        )
        {
            _book.SetBookData(
                new BookData(
                    publisher,
                    yearOfPublication,
                    pages,
                    dateProvider
                )
            );
            return this;
        }

        public Book GetBook() => _book;
    }
}