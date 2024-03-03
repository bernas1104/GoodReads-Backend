namespace GoodReads.Application.Common.Pagination
{
    public abstract record PaginatedResponse<TResponse>(
        IEnumerable<TResponse> Data,
        int CurrentPage,
        int TotalItens,
        int TotalPages,
        int PageSize
    );
}