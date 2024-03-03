using System.Linq.Expressions;

using ErrorOr;

using GoodReads.Application.Features.Users.Create;
using GoodReads.Domain.Common.Exceptions;
using GoodReads.Domain.Common.Interfaces.Repositories.EntityFramework;
using GoodReads.Domain.UserAggregate.Entities;
using GoodReads.Domain.UserAggregate.ValueObjects;
using GoodReads.Shared.Mocks;
using GoodReads.Unit.Tests.Helpers;

using Microsoft.Extensions.Logging;

namespace GoodReads.Unit.Tests.Application.Features.Users.Create
{
    public class CreateUserRequestHandlerTest
    {
        private readonly IRepository<User, UserId, Guid> _repository;
        private readonly ILogger<CreateUserRequestHandler> _logger;
        private readonly CreateUserRequestHandler _handler;

        public CreateUserRequestHandlerTest()
        {
            _repository = Substitute.For<IRepository<User, UserId, Guid>>();
            _logger = Substitute.For<ILogger<CreateUserRequestHandler>>();
            _handler = new (_repository, _logger);
        }

        [Fact]
        public async void GivenCreateUserRequest_WhenEmailAlreadyUsed_ShouldReturnErrorConflict()
        {
            // arrange
            var request = UserMock.GetCreateUserRequest();
            var email = request.Email;

            _repository.GetByFilterAsync(
                Arg.Any<Expression<Func<User, bool>>>(),
                Arg.Any<CancellationToken>()
            ).Returns(UserMock.Get());

            // act
            var response = await _handler.Handle(request, CancellationToken.None);

            // assert
            response.Should().NotBeNull();
            response!.IsError.Should().BeTrue();

            await _repository.DidNotReceive()
                .AddAsync(
                    Arg.Any<User>(),
                    Arg.Any<CancellationToken>()
                );

            _logger.ShouldHaveLoggedError(
                $"E-mail {request.Email} is already being used"
            );
        }

        [Fact]
        public async Task GivenCreateUserRequest_WhenEmailNotUsed_ShouldReturnCreatedUserId()
        {
            // arrange
            var request = UserMock.GetCreateUserRequest();
            var email = request.Email;

            _repository.GetByFilterAsync(
                Arg.Any<Expression<Func<User, bool>>>(),
                Arg.Any<CancellationToken>()
            ).Returns((User?)null);

            // act
            var response = await _handler.Handle(request, CancellationToken.None);

            // assert
            response.Should().NotBeNull();
            response!.Value.Should().NotBeEmpty();

            await _repository.Received()
                .AddAsync(
                    Arg.Is<User>(
                        u => u.Name == request.Name &&
                            u.Email == request.Email
                    ),
                    Arg.Any<CancellationToken>()
                );

            _logger.ShouldHaveLoggedInformation(
                $"User {request.Name} was registered successfully"
            );
        }
    }
}