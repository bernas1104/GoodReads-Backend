using ErrorOr;

using GoodReads.Application.Common.Repositories.EntityFramework;
using GoodReads.Domain.UserAggregate.Entities;
using GoodReads.Domain.UserAggregate.ValueObjects;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GoodReads.Application.Features.Users.Create
{
    public sealed class CreateUserRequestHandler :
        IRequestHandler<CreateUserRequest, ErrorOr<Guid>>
    {
        private readonly IRepository<User, UserId, Guid> _repository;
        private readonly ILogger<CreateUserRequestHandler> _logger;

        public CreateUserRequestHandler(
            IRepository<User, UserId, Guid> repository,
            ILogger<CreateUserRequestHandler> logger
        )
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ErrorOr<Guid>> Handle(
            CreateUserRequest request,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _repository.GetByFilterAsync(
                u => u.Email == request.Email &&
                    u.DeletedAt == null,
                cancellationToken
            );

            if (user is not null)
            {
                return EmailAlreadyUsedError(request.Email);
            }

            user = new User(request.Name, request.Email);

            await RegisterUserAsync(user, cancellationToken);

            return user.Id.Value;
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

        private async Task RegisterUserAsync(User user, CancellationToken cancellationToken)
        {
            await _repository.AddAsync(user, cancellationToken);

            _logger.LogInformation(
                "User {Name} was registered successfully",
                user.Name
            );
        }
    }
}