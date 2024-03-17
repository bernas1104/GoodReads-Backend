using GoodReads.Domain.Common.Interfaces.Providers;

namespace GoodReads.Domain.BookAggregate.Interfaces.Builders
{
    public interface IBookBuilder
    {
        IBookBuilder AddDescription(string description);
        IBookBuilder AddBookData(
            string publisher,
            int yearOfPublication,
            int pages
        );
    }
}