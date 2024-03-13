using ErrorOr;

using GoodReads.Application.Common.Repositories.EntityFramework;
using GoodReads.Domain.UserAggregate.Entities;
using GoodReads.Domain.UserAggregate.ValueObjects;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GoodReads.Application.Features.Users.Update
{
    public sealed class UpdateUserHandler :
        IRequestHandler<UpdateUserRequest, ErrorOr<Updated>>
    {
        private readonly IRepository<User, UserId, Guid> _repository;
        private readonly ILogger<UpdateUserHandler> _logger;

        public UpdateUserHandler(
            IRepository<User, UserId, Guid> repository,
            ILogger<UpdateUserHandler> logger
        )
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ErrorOr<Updated>> Handle(
            UpdateUserRequest request,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var userId = UserId.Create(request.Id);
            var user = await _repository.GetByIdAsync(
                userId,
                cancellationToken
            );

            if (user is null)
            {
                return UserNotFoundError(request);
            }

            var differentUser = await _repository.GetByFilterAsync(
                u => !u.Id.Equals(userId) &&
                    u.Email == request.Email &&
                    u.DeletedAt == null,
                cancellationToken
            );

            if (differentUser is not null)
            {
                return EmailAlreadyUsedError(request.Email);
            }

            user.Update(request.Name, request.Email);

            await _repository.UpdateAsync(user, cancellationToken);

            _logger.LogInformation(
                "User ({UserId}) was updated successfully",
                user.Id.Value
            );

            return Result.Updated;
        }

        private Error UserNotFoundError(UpdateUserRequest request)
        {
            _logger.LogError(
                "User ({UserId}) was not found",
                request.Id
            );

            return Error.NotFound(
                code: "User.NotFound",
                description: $"User ({request.Id}) was not found"
            );
        }

        private Error EmailAlreadyUsedError(string email)
        {
            _logger.LogError(
                "E-mail {Email} is already being used",
                email
            );

            return Error.Conflict(
                code: "User.Conflict",
                description: $"E-mail {email} is already being used"
            );
        }
    }
}