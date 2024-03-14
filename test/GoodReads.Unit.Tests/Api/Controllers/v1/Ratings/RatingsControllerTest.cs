using System.Net;

using ErrorOr;

using GoodReads.Api.Controllers.v1.Ratings;
using GoodReads.Application.Common.Pagination;
using GoodReads.Application.Features.Ratings;
using GoodReads.Application.Features.Ratings.Delete;
using GoodReads.Application.Features.Ratings.GetById;
using GoodReads.Application.Features.Ratings.GetPaginated;
using GoodReads.Application.Features.Ratings.Update;
using GoodReads.Shared.Mocks;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace GoodReads.Unit.Tests.Api.Controllers.v1.Ratings
{
    public class RatingsControllerTest
    {
        private readonly ISender _sender;
        private readonly RatingsController _controller;

        public RatingsControllerTest()
        {
            _sender = Substitute.For<ISender>();
            _controller = new (_sender);
        }

        [Fact]
        public async Task GivenCreateAsync_WhenValidRequest_ShouldReturnCreated()
        {
            // arrange
            var ratingId = Guid.NewGuid();
            var request = RatingMock.GetCreateRatingRequest();

            _sender.Send(
                Arg.Any<CreateRatingRequest>(),
                Arg.Any<CancellationToken>()
            ).Returns(ratingId);

            // act
            var response = await _controller.CreateAsync(
                request,
                CancellationToken.None
            ) as CreatedAtActionResult;

            // assert
            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((int)HttpStatusCode.Created);
            response!.Value.Should().BeOfType<ErrorOr<Guid>>();
            ((ErrorOr<Guid>)response!.Value!).Value.Should().Be(ratingId);
        }

        [Fact]
        public async Task GivenGetPaginatedAsync_ShouldReturnOk()
        {
            // arrange
            var request = RatingMock.GetPaginatedRatingsRequest();
            var ratings = RatingMock.GetFakeRatingResponse()
                .GenerateBetween(1, 10);
            var ratingsCount = ratings.Count;

            _sender.Send(
                Arg.Any<GetPaginatedRatingsRequest>(),
                Arg.Any<CancellationToken>()
            ).Returns(PaginationMock.GetPaginatedResponse(ratings));

            // act
            var response = await _controller.GetPaginatedAsync(
                request,
                CancellationToken.None
            ) as OkObjectResult;

            // assert
            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((int)HttpStatusCode.OK);
            response!.Value.Should().BeOfType<PaginatedResponse<RatingResponse>>();
            ((PaginatedResponse<RatingResponse>)response!.Value!).Data.Should()
                .HaveCount(ratingsCount);
        }

        [Fact]
        public async Task GivenGetByIdAsync_WhenRatingFound_ShouldReturnOk()
        {
            // arrange
            var ratingId = Guid.NewGuid();

            _sender.Send(
                Arg.Any<GetRatingByIdRequest>(),
                Arg.Any<CancellationToken>()
            ).Returns(RatingMock.GetFakeRatingResponse().Generate());

            // act
            var response = await _controller.GetByIdAsync(
                ratingId,
                CancellationToken.None
            ) as OkObjectResult;

            // assert
            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((int)HttpStatusCode.OK);
            response!.Value.Should().BeOfType<ErrorOr<RatingResponse>>();
        }

        [Fact]
        public async Task GivenUpdateAsync_WhenValidRequest_ShouldReturnNoContent()
        {
            // arrange
            var ratingId = Guid.NewGuid();
            var request = new UpdateRatingRequest(ratingId, "SomeDescription");

            _sender.Send(
                Arg.Any<UpdateRatingRequest>(),
                Arg.Any<CancellationToken>()
            ).Returns(Result.Updated);

            // act
            var response = await _controller.UpdateAsync(
                ratingId,
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
            var ratingId = Guid.NewGuid();
            var request = new UpdateRatingRequest(ratingId, "SomeDescription");

            _sender.Send(
                Arg.Any<UpdateRatingRequest>(),
                Arg.Any<CancellationToken>()
            ).Returns(Error.Failure());

            // act
            var response = await _controller.UpdateAsync(
                ratingId,
                request,
                CancellationToken.None
            ) as BadRequestObjectResult;

            // assert
            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((ErrorOr<Updated>)response!.Value!).IsError.Should().BeTrue();
        }

        [Fact]
        public async Task GivenUpdateAsync_WhenRouteIdDifferentFromBodyId_ShouldReturnBadRequest()
        {
            // arrange
            var ratingId = Guid.NewGuid();
            var request = new UpdateRatingRequest(Guid.NewGuid(), "SomeDescription");

            // act
            var response = await _controller.UpdateAsync(
                ratingId,
                request,
                CancellationToken.None
            ) as BadRequestObjectResult;

            // assert
            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((ErrorOr<Updated>)response!.Value!).IsError.Should().BeTrue();
        }

        [Fact]
        public async Task GivenDeleteAsync_WhenRatingDeleted_ShouldReturnNoContent()
        {
            // arrange
            var ratingId = Guid.NewGuid();

            _sender.Send(
                Arg.Any<DeleteRatingRequest>(),
                Arg.Any<CancellationToken>()
            ).Returns(Result.Deleted);

            // act
            var response = await _controller.DeleteAsync(
                ratingId,
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
            var ratingId = Guid.NewGuid();

            _sender.Send(
                Arg.Any<DeleteRatingRequest>(),
                Arg.Any<CancellationToken>()
            ).Returns(Error.Failure());

            // act
            var response = await _controller.DeleteAsync(
                ratingId,
                CancellationToken.None
            ) as BadRequestObjectResult;

            // assert
            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((ErrorOr<Deleted>)response!.Value!).IsError.Should().BeTrue();
        }
    }
}