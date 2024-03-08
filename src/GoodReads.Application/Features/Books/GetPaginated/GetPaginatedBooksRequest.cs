using GoodReads.Application.Common.Pagination;

using MediatR;

namespace GoodReads.Application.Features.Books.GetPaginated
{
    public record GetPaginatedBooksRequest(int Page, int Size) :
        PaginatedRequest(Page, Size),
        IRequest<PaginatedResponse<BookResponse>>;
}