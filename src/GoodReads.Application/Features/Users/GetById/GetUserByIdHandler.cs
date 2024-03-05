using ErrorOr;

using GoodReads.Domain.Common.Interfaces.Repositories.EntityFramework;
using GoodReads.Domain.UserAggregate.Entities;
using GoodReads.Domain.UserAggregate.ValueObjects;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GoodReads.Application.Features.Users.GetById
{
    public sealed class GetUserByIdHandler :
        IRequestHandler<GetUserByIdRequest, ErrorOr<GetUserByIdResponse>>
    {
        private readonly IRepository<User, UserId, Guid> _repository;
        private readonly ILogger<GetUserByIdHandler> _logger;

        public GetUserByIdHandler(
            IRepository<User, UserId, Guid> repository,
            ILogger<GetUserByIdHandler> logger
        )
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ErrorOr<GetUserByIdResponse>> Handle(
            GetUserByIdRequest request,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _repository.GetByIdAsync(
                UserId.Create(request.Id),
                cancellationToken
            );

            if (user is null)
            {
                return UserNotFoundError(request);
            }

            return new GetUserByIdResponse(
                user.Name,
                user.Email,
                user.RatingIds.Count,
                new UserRatingIdsResponse(user.RatingIds.Select(r => r.Value)),
                user.CreatedAt
            );
        }

        private Error UserNotFoundError(GetUserByIdRequest request)
        {
            _logger.LogWarning(
                "User ({UserId}) was not found",
                request.Id
            );

            return Error.NotFound(
                code: "User.NotFound",
                description: $"User ({request.Id}) was not found"
            );
        }
    }
}