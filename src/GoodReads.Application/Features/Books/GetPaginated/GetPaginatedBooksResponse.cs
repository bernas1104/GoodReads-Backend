using GoodReads.Application.Common.Pagination;

namespace GoodReads.Application.Features.Books.GetPaginated
{
    public record GetPaginatedBooksResponse(
        IEnumerable<BookResponse> Data,
        int CurrentPage,
        int TotalItens,
        int TotalPages,
        int PageSize
    ) : PaginatedResponse<BookResponse>(
        Data,
        CurrentPage,
        TotalItens,
        TotalPages,
        PageSize
    );
}