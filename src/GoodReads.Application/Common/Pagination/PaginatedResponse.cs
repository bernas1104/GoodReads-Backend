namespace GoodReads.Application.Common.Pagination
{
    public sealed record PaginatedResponse<TResponse>(
        IEnumerable<TResponse> Data,
        int CurrentPage,
        int TotalItens,
        int TotalPages,
        int PageSize
    );
}