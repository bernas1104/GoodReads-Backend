namespace GoodReads.Application.Common.Pagination
{
    public abstract record PaginatedRequest(int Page, int Size);
}