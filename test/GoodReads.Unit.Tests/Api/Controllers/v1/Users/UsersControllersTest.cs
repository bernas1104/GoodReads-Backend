using System.Net;

using ErrorOr;

using GoodReads.Api.Controllers.v1.Users;
using GoodReads.Application.Common.Pagination;
using GoodReads.Application.Features.Users;
using GoodReads.Application.Features.Users.Create;
using GoodReads.Application.Features.Users.Delete;
using GoodReads.Application.Features.Users.GetById;
using GoodReads.Application.Features.Users.GetPaginated;
using GoodReads.Application.Features.Users.Update;
using GoodReads.Shared.Mocks;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace GoodReads.Unit.Tests.Api.Controllers.v1.Users
{
    public class UsersControllersTest
    {
        private readonly ISender _sender;
        private readonly UsersController _controller;

        public UsersControllersTest()
        {
            _sender = Substitute.For<ISender>();
            _controller = new (_sender);
        }

        [Fact]
        public async Task GivenCreateAsync_WhenValidRequest_ShouldReturnCreated()
        {
            // arrange
            var userId = Guid.NewGuid();
            var request = UserMock.GetCreateUserRequest();

            _sender.Send(
                Arg.Is<CreateUserRequest>(
                    x => x.Name == request.Name &&
                        x.Email == request.Email
                ),
                Arg.Any<CancellationToken>()
            ).Returns(userId);

            // act
            var response = await _controller.CreateAsync(
                request,
                CancellationToken.None
            ) as CreatedAtActionResult;

            // assert
            response.Should().NotBeNull();
            response!.Value.Should().BeOfType<ErrorOr<Guid>>();
            ((ErrorOr<Guid>)response!.Value!).Value.Should().Be(userId);
        }

        [Fact]
        public async Task GivenGetPaginatedAsync_ShouldReturnOk()
        {
            // arrange
            var request = UserMock.GetPaginatedUsersRequest();
            var users = UserMock.GetUserResponse().GenerateBetween(5, 10);
            var usersCount = users.Count;

            _sender.Send(
                Arg.Is<GetPaginatedUsersRequest>(
                    x => x.Page == request.Page && x.Size == request.Size
                ),
                Arg.Any<CancellationToken>()
            ).Returns(PaginationMock.GetPaginatedResponse(users));

            // act
            var response = await _controller.GetPaginatedAsync(
                request.Page,
                request.Size,
                CancellationToken.None
            ) as OkObjectResult;

            // assert
            response.Should().NotBeNull();
            response!.Value.Should().BeOfType<PaginatedResponse<UserResponse>>();
            ((PaginatedResponse<UserResponse>)response!.Value!).Data.Should()
                .HaveCount(usersCount);
        }

        [Fact]
        public async Task GivenGetByIdAsync_WhenUserFound_ShouldReturnOk()
        {
            // arrange
            var request = new GetUserByIdRequest(Guid.NewGuid());

            _sender.Send(
                Arg.Is<GetUserByIdRequest>(x => x.Id == request.Id),
                Arg.Any<CancellationToken>()
            ).Returns(UserMock.GetUserByIdResponse());

            // act
            var response = await _controller.GetByIdAsync(
                request.Id,
                CancellationToken.None
            ) as OkObjectResult;

            // assert
            response.Should().NotBeNull();
            response!.Value.Should().BeOfType<ErrorOr<GetUserByIdResponse>>();
        }

        [Fact]
        public async Task GivenUpdateAsync_WhenValidRequest_ShouldReturnNoContent()
        {
            // arrange
            var userId = Guid.NewGuid();
            var request = UserMock.GetUpdateUserRequest(userId);

            _sender.Send(
                Arg.Is<UpdateUserRequest>(
                    x => x.Id == request.Id
                ),
                Arg.Any<CancellationToken>()
            ).Returns(Result.Updated);

            // act
            var response = await _controller.UpdateAsync(
                userId,
                request,
                CancellationToken.None
            ) as NoContentResult;

            // assert
            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task GivenUpdateAsync_WhenRequestError_ShouldReturnBadRequest()
        {
            // arrange
            var userId = Guid.NewGuid();
            var request = UserMock.GetUpdateUserRequest(userId);

            _sender.Send(
                Arg.Any<UpdateUserRequest>(),
                Arg.Any<CancellationToken>()
            ).Returns(Error.Failure());

            // act
            var response = await _controller.UpdateAsync(
                userId,
                request,
                CancellationToken.None
            ) as BadRequestObjectResult;

            // assert
            response.Should().NotBeNull();
            response!.Value.Should().BeOfType<ErrorOr<Updated>>();
            ((ErrorOr<Updated>)response!.Value!).IsError.Should().BeTrue();
        }

        [Fact]
        public async Task GivenUpdateAsync_WhenRouteIdDifferentFromBodyId_ShouldReturnBadRequest()
        {
            // arrange
            var userId = Guid.NewGuid();
            var request = UserMock.GetUpdateUserRequest();

            // act
            var response = await _controller.UpdateAsync(
                userId,
                request,
                CancellationToken.None
            ) as BadRequestObjectResult;

            // assert
            response.Should().NotBeNull();
            response!.Value.Should().BeOfType<ErrorOr<Updated>>();
            ((ErrorOr<Updated>)response!.Value!).IsError.Should().BeTrue();
        }

        [Fact]
        public async Task GivenDeleteAsync_WhenUserDeleted_ShouldReturnNoContent()
        {
            // arrange
            var request = new DeleteUserRequest(Guid.NewGuid());

            _sender.Send(
                Arg.Is<DeleteUserRequest>(x => x.Id == request.Id),
                Arg.Any<CancellationToken>()
            ).Returns(Result.Deleted);

            // act
            var response = await _controller.DeleteAsync(
                request.Id,
                CancellationToken.None
            ) as NoContentResult;

            // assert
            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task GivenDeleteAsync_WhenRequestError_ShouldReturnBadRequest()
        {
            // arrange
            var request = new DeleteUserRequest(Guid.NewGuid());

            _sender.Send(
                Arg.Is<DeleteUserRequest>(x => x.Id == request.Id),
                Arg.Any<CancellationToken>()
            ).Returns(Error.Failure());

            // act
            var response = await _controller.DeleteAsync(
                request.Id,
                CancellationToken.None
            ) as BadRequestObjectResult;

            // assert
            response.Should().NotBeNull();
            response!.Value.Should().BeOfType<ErrorOr<Deleted>>();
            ((ErrorOr<Deleted>)response!.Value!).IsError.Should().BeTrue();
        }
    }
}