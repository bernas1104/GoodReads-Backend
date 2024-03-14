using GoodReads.Application.Common.Pagination;

using MediatR;

namespace GoodReads.Application.Features.Users.GetPaginated
{
    public sealed record GetPaginatedUsersRequest(int Page, int Size) :
        PaginatedRequest(Page, Size),
        IRequest<PaginatedResponse<UserResponse>>;
}