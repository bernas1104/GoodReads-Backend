using GoodReads.Domain.Common.Interfaces.Providers;

namespace GoodReads.Domain.Books.Interfaces.Builders
{
    public interface IBookBuilder
    {
        IBookBuilder AddDescription(string description);
        IBookBuilder AddCover(IEnumerable<byte> cover);
        IBookBuilder AddBookData(
            string publisher,
            int yearOfPublication,
            int pages,
            IDateProvider dateProvider
        );
    }
}