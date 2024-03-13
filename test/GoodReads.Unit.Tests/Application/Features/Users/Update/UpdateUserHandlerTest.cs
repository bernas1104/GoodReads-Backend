using System.Linq.Expressions;

using ErrorOr;

using GoodReads.Application.Features.Users.Update;
using GoodReads.Application.Common.Repositories.EntityFramework;
using GoodReads.Domain.UserAggregate.Entities;
using GoodReads.Domain.UserAggregate.ValueObjects;
using GoodReads.Shared.Mocks;
using GoodReads.Unit.Tests.Helpers;

using Microsoft.Extensions.Logging;

namespace GoodReads.Unit.Tests.Application.Features.Users.Update
{
    public class UpdateUserHandlerTest
    {
        private readonly IRepository<User, UserId, Guid> _repository;
        private readonly ILogger<UpdateUserHandler> _logger;
        private readonly UpdateUserHandler _handler;

        public UpdateUserHandlerTest()
        {
            _repository = Substitute.For<IRepository<User, UserId, Guid>>();
            _logger = Substitute.For<ILogger<UpdateUserHandler>>();
            _handler = new (_repository, _logger);
        }

        [Fact]
        public async Task GivenUpdateUserRequest_WhenUserNotFound_ShouldReturnErrorNotFound()
        {
            // arrange
            var request = UserMock.GetUpdateUserRequest();

            _repository.GetByIdAsync(
                Arg.Is<UserId>(x => x.Equals(UserId.Create(request.Id))),
                Arg.Any<CancellationToken>()
            ).Returns((User?)null);

            // act
            var response = await _handler.Handle(request, CancellationToken.None);

            // assert
            response.Should().NotBeNull();
            response.IsError.Should().BeTrue();
            response.FirstError.Code.Should().Be("User.NotFound");

            await _repository.DidNotReceive().
                UpdateAsync(
                    Arg.Any<User>(),
                    Arg.Any<CancellationToken>()
                );

            _logger.ShouldHaveLoggedError($"User ({request.Id}) was not found");
        }

        [Fact]
        public async Task GivenUpdateUserRequest_WhenDifferentUserWithSameEmailFound_ShouldReturnErrorConflict()
        {
            // arrange
            var request = UserMock.GetUpdateUserRequest();
            var user = UserMock.Get(UserId.Create(request.Id));
            var differentUser = UserMock.Get(email: request.Email);

            _repository.GetByIdAsync(
                Arg.Is<UserId>(x => x.Equals(UserId.Create(request.Id))),
                Arg.Any<CancellationToken>()
            ).Returns(user);

            _repository.GetByFilterAsync(
                Arg.Any<Expression<Func<User, bool>>>(),
                Arg.Any<CancellationToken>()
            ).Returns(differentUser);

            // act
            var response = await _handler.Handle(request, CancellationToken.None);

            // assert
            response.Should().NotBeNull();
            response.IsError.Should().BeTrue();
            response.FirstError.Code.Should().Be("User.Conflict");

            await _repository.DidNotReceive().
                UpdateAsync(
                    Arg.Any<User>(),
                    Arg.Any<CancellationToken>()
                );

            _logger.ShouldHaveLoggedError($"E-mail {request.Email} is already being used");
        }

        [Fact]
        public async Task GivenUpdateUserRequest_WhenUserFoundAndEmailNotUsed_ShouldReturnUpdated()
        {
            // arrange
            var request = UserMock.GetUpdateUserRequest();
            var user = UserMock.Get(UserId.Create(request.Id));

            _repository.GetByIdAsync(
                Arg.Is<UserId>(x => x.Equals(UserId.Create(request.Id))),
                Arg.Any<CancellationToken>()
            ).Returns(user);

            _repository.GetByFilterAsync(
                Arg.Any<Expression<Func<User, bool>>>(),
                Arg.Any<CancellationToken>()
            ).Returns((User?)null);

            // act
            var response = await _handler.Handle(request, CancellationToken.None);

            // assert
            response.Should().NotBeNull();
            response.IsError.Should().BeFalse();
            response.Value.Should().Be(Result.Updated);

            await _repository.Received().
                UpdateAsync(
                    Arg.Is<User>(u => u.Id.Equals(user.Id)),
                    Arg.Any<CancellationToken>()
                );

            _logger.ShouldHaveLoggedInformation($"User ({user.Id.Value}) was updated successfully");
        }
    }
}