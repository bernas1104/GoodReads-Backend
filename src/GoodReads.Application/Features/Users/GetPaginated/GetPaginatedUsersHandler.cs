using GoodReads.Application.Common.Pagination;
using GoodReads.Application.Common.Repositories.EntityFramework;
using GoodReads.Domain.Common.EntityFramework;
using GoodReads.Domain.UserAggregate.Entities;
using GoodReads.Domain.UserAggregate.ValueObjects;

using MediatR;

namespace GoodReads.Application.Features.Users.GetPaginated
{
    public sealed class GetPaginatedUsersHandler :
        IRequestHandler<GetPaginatedUsersRequest, PaginatedResponse<UserResponse>>
    {
        private readonly IRepository<User, UserId, Guid> _repository;

        public GetPaginatedUsersHandler(IRepository<User, UserId, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<PaginatedResponse<UserResponse>> Handle(
            GetPaginatedUsersRequest request,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var users = await _repository.GetPaginatedAsync(
                null,
                request.Page,
                request.Size,
                cancellationToken
            );

            var count = await _repository.GetCountAsync(cancellationToken);

            return new PaginatedResponse<UserResponse>(
                Data: users.Select(u => new UserResponse(u.Name, u.RatingIds.Count)),
                CurrentPage: request.Page,
                TotalItens: count,
                TotalPages: (int)Math.Ceiling(count / (decimal)request.Size),
                PageSize: request.Size
            );
        }
    }
}