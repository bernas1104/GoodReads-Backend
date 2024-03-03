using GoodReads.Domain.Common.Interfaces.Repositories.EntityFramework;
using GoodReads.Domain.UserAggregate.Entities;
using GoodReads.Domain.UserAggregate.ValueObjects;

using MediatR;

namespace GoodReads.Application.Features.Users.GetPaginated
{
    public sealed class GetPaginatedUsersHandler :
        IRequestHandler<GetPaginatedUsersRequest, GetPaginatedUsersResponse>
    {
        private readonly IRepository<User, UserId, Guid> _repository;

        public GetPaginatedUsersHandler(IRepository<User, UserId, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<GetPaginatedUsersResponse> Handle(
            GetPaginatedUsersRequest request,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var users = await _repository.GetPaginatedAsync(
                request.Page,
                request.Size,
                cancellationToken
            );

            var count = await _repository.GetCountAsync(cancellationToken);

            return new GetPaginatedUsersResponse(
                Data: users.Select(
                    u => new GetPaginatedUserResponse(u.Name, u.RatingIds.Count)
                ),
                CurrentPage: request.Page,
                TotalItens: count,
                TotalPages: count / request.Size,
                PageSize: request.Size
            );
        }
    }
}