using GoodReads.Application.Features.Users.Delete;
using GoodReads.Domain.Common.Interfaces.Repositories.EntityFramework;
using GoodReads.Domain.UserAggregate.Entities;
using GoodReads.Domain.UserAggregate.ValueObjects;
using GoodReads.Shared.Mocks;
using GoodReads.Unit.Tests.Helpers;

using Microsoft.Extensions.Logging;

namespace GoodReads.Unit.Tests.Application.Features.Users.Delete
{
    public class DeleteUserHandlerTest
    {
        private readonly IRepository<User, UserId, Guid> _repository;
        private readonly ILogger<DeleteUserHandler> _logger;
        private readonly DeleteUserHandler _handler;

        public DeleteUserHandlerTest()
        {
            _repository = Substitute.For<IRepository<User, UserId, Guid>>();
            _logger = Substitute.For<ILogger<DeleteUserHandler>>();
            _handler = new (_repository, _logger);
        }

        [Fact]
        public async Task GivenDeleteUserRequest_WhenUserNotFound_ShouldReturnErrorNotFound()
        {
            // arrange
            var request = new DeleteUserRequest(Guid.NewGuid());

            _repository.GetByIdAsync(
                Arg.Is<UserId>(x => x.Equals(UserId.Create(request.Id))),
                Arg.Any<CancellationToken>()
            ).Returns((User?)null);

            // act
            var response = await _handler.Handle(request, CancellationToken.None);

            // assert
            response.Should().NotBeNull();
            response!.IsError.Should().BeTrue();
            response!.FirstError.Code.Should().Be("User.NotFound");

            _logger.ShouldHaveLoggedWarning($"User ({request.Id}) was not found");
        }

        [Fact]
        public async Task GivenDeleteUserRequest_WhenUserFound_ShouldDeleteUser()
        {
            // arrange
            var request = new DeleteUserRequest(Guid.NewGuid());
            var user = UserMock.Get(UserId.Create(request.Id));

            _repository.GetByIdAsync(
                Arg.Is<UserId>(x => x.Equals(UserId.Create(request.Id))),
                Arg.Any<CancellationToken>()
            ).Returns(user);

            // act
            var response = await _handler.Handle(request, CancellationToken.None);

            // assert
            response.Should().NotBeNull();
            response!.IsError.Should().BeFalse();

            await _repository.Received()
                .UpdateAsync(
                    Arg.Is<User>(u => u.Id.Equals(user.Id)),
                    Arg.Any<CancellationToken>()
                );

            _logger.ShouldHaveLoggedInformation($"User ({request.Id}) was deleted");
        }
    }
}