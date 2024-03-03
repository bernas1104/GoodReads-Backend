using GoodReads.Application.Features.Users.GetById;
using GoodReads.Domain.Common.Interfaces.Repositories.EntityFramework;
using GoodReads.Domain.UserAggregate.Entities;
using GoodReads.Domain.UserAggregate.ValueObjects;
using GoodReads.Shared.Mocks;
using GoodReads.Unit.Tests.Helpers;

using Microsoft.Extensions.Logging;

namespace GoodReads.Unit.Tests.Application.Features.Users.GetById
{
    public class GetUserByIdHandlerTest
    {
        private readonly IRepository<User, UserId, Guid> _repository;
        private readonly ILogger<GetUserByIdHandler> _logger;
        private readonly GetUserByIdHandler _handler;

        public GetUserByIdHandlerTest()
        {
            _repository = Substitute.For<IRepository<User, UserId, Guid>>();
            _logger = Substitute.For<ILogger<GetUserByIdHandler>>();
            _handler = new (_repository, _logger);
        }

        [Fact]
        public async Task GivenGetUserByIdRequest_WhenUserNotFound_ShouldReturnErrorNotFound()
        {
            // arrange
            var request = new GetUserByIdRequest(Guid.NewGuid());

            _repository.GetByIdAsync(
                Arg.Is<UserId>(u => u.Equals(UserId.Create(request.Id))),
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
        public async Task GivenGetUserByIdRequest_WhenUserFound_ShouldReturnUserResponse()
        {
            // arrange
            var user = UserMock.Get();
            var request = new GetUserByIdRequest(Guid.NewGuid());

            _repository.GetByIdAsync(
                Arg.Is<UserId>(u => u.Equals(UserId.Create(request.Id))),
                Arg.Any<CancellationToken>()
            ).Returns(user);

            // act
            var response = await _handler.Handle(request, CancellationToken.None);

            // assert
            response.Should().NotBeNull();
            response!.IsError.Should().BeFalse();
            response!.Value.Name.Should().Be(user.Name);
            response!.Value.Email.Should().Be(user.Email);
        }
    }
}