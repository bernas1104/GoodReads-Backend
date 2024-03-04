using ErrorOr;

using GoodReads.Domain.Common.Interfaces.Repositories.EntityFramework;
using GoodReads.Domain.UserAggregate.Entities;
using GoodReads.Domain.UserAggregate.ValueObjects;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GoodReads.Application.Features.Users.Delete
{
    public sealed class DeleteUserHandler :
        IRequestHandler<DeleteUserRequest, ErrorOr<Deleted>>
    {
        private readonly IRepository<User, UserId, Guid> _repository;
        private readonly ILogger<DeleteUserHandler> _logger;

        public DeleteUserHandler(
            IRepository<User, UserId, Guid> repository,
            ILogger<DeleteUserHandler> logger
        )
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ErrorOr<Deleted>> Handle(
            DeleteUserRequest request,
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

            user.Delete();

            await _repository.UpdateAsync(user, cancellationToken);

            _logger.LogInformation("User ({UserId}) was deleted", user.Id.Value);

            return Result.Deleted;
        }

        private Error UserNotFoundError(DeleteUserRequest request)
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