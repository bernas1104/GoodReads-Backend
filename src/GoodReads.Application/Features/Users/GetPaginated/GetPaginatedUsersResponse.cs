using GoodReads.Application.Common.Pagination;

namespace GoodReads.Application.Features.Users.GetPaginated
{
    public sealed record GetPaginatedUsersResponse(
        IEnumerable<GetPaginatedUserResponse> Data,
        int CurrentPage,
        int TotalItens,
        int TotalPages,
        int PageSize
    ) : PaginatedResponse<GetPaginatedUserResponse>(
        Data,
        CurrentPage,
        TotalItens,
        TotalPages,
        PageSize
    );

    public sealed record GetPaginatedUserResponse(string Name, int TotalRatings) :
        UserResponse(Name, TotalRatings);
}