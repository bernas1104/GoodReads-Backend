using GoodReads.Domain.Common.Interfaces.Providers;

namespace GoodReads.Domain.Books.Interfaces.Builders
{
    public interface IBookBuilder
    {
        IBookBuilder AddDescription(string description);
        IBookBuilder AddPublisher(string publisher);
        IBookBuilder AddYearOfPublication(int yearOfPublication, IDateProvider dateProvider);
        IBookBuilder AddPages(int pages);
        IBookBuilder AddCover(IEnumerable<byte> cover);
    }
}