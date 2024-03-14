using System.Net;

using ErrorOr;

using GoodReads.Api.Controllers.v1.Books;
using GoodReads.Application.Common.Pagination;
using GoodReads.Application.Features.Books;
using GoodReads.Application.Features.Books.Create;
using GoodReads.Application.Features.Books.Delete;
using GoodReads.Application.Features.Books.GetById;
using GoodReads.Application.Features.Books.GetPaginated;
using GoodReads.Application.Features.Books.Update;
using GoodReads.Shared.Mocks;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace GoodReads.Unit.Tests.Api.Controllers.v1.Books
{
    public class BooksControllerTest
    {
        private readonly ISender _sender;
        private readonly BooksController _controller;

        public BooksControllerTest()
        {
            _sender = Substitute.For<ISender>();
            _controller = new (_sender);
        }

        [Fact]
        public async Task GivenCreateAsync_WhenValidRequest_ShouldReturnCreated()
        {
            // arrange
            var bookId = Guid.NewGuid();
            var request = BookMock.GetCreateBookRequest();

            _sender.Send(
                Arg.Any<CreateBookRequest>(),
                Arg.Any<CancellationToken>()
            ).Returns(bookId);

            // act
            var response = await _controller.CreateAsync(
                request,
                CancellationToken.None
            ) as CreatedAtActionResult;

            // assert
            response.Should().NotBeNull();
            response!.Value.Should().BeOfType<ErrorOr<Guid>>();
            ((ErrorOr<Guid>)response!.Value!).Value.Should().Be(bookId);
        }

        [Fact]
        public async Task GivenGetPaginatedAsync_ShouldReturnOk()
        {
            // arrange
            var size = new Faker().Random.Int(5, 10);
            var books = BookMock.GetFakeBookResponse()
                .GenerateBetween(size, size);
            var request = new GetPaginatedBooksRequest(1, 10);

            _sender.Send(
                Arg.Any<GetPaginatedBooksRequest>(),
                Arg.Any<CancellationToken>()
            ).Returns(PaginationMock.GetPaginatedResponse(books));

            // act
            var result = await _controller.GetPaginatedAsync(
                request,
                CancellationToken.None
            ) as OkObjectResult;

            // assert
            result.Should().NotBeNull();
            result!.Value.Should().BeOfType<PaginatedResponse<BookResponse>>();
            ((PaginatedResponse<BookResponse>)result!.Value!).Data.Should()
                .HaveCount(size);
        }

        [Fact]
        public async Task GivenGetByIdAsync_WhenUserFound_ShouldReturnOk()
        {
            // arrange
            var request = new GetBookByIdRequest(Guid.NewGuid());

            _sender.Send(
                Arg.Is<GetBookByIdRequest>(x => x.Id == request.Id),
                Arg.Any<CancellationToken>()
            ).Returns(BookMock.GetBookByIdResponse());

            // act
            var response = await _controller.GetByIdAsync(
                request.Id,
                CancellationToken.None
            ) as OkObjectResult;

            // assert
            response.Should().NotBeNull();
            response!.Value.Should().BeOfType<ErrorOr<GetBookByIdResponse>>();
        }

        [Fact]
        public async Task GivenUpdateAsync_WhenValidRequest_ShouldReturnNoContent()
        {
            // arrange
            var bookId = Guid.NewGuid();
            var request = BookMock.GetUpdateBookRequest(bookId);

            _sender.Send(
                Arg.Is<UpdateBookRequest>(
                    x => x.Id == request.Id
                ),
                Arg.Any<CancellationToken>()
            ).Returns(Result.Updated);

            // act
            var response = await _controller.UpdateAsync(
                bookId,
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
            var bookId = Guid.NewGuid();
            var request = BookMock.GetUpdateBookRequest(bookId);

            _sender.Send(
                Arg.Any<UpdateBookRequest>(),
                Arg.Any<CancellationToken>()
            ).Returns(Error.Failure());

            // act
            var response = await _controller.UpdateAsync(
                bookId,
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
            var bookId = Guid.NewGuid();
            var request = BookMock.GetUpdateBookRequest();

            // act
            var response = await _controller.UpdateAsync(
                bookId,
                request,
                CancellationToken.None
            ) as BadRequestObjectResult;

            // assert
            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((ErrorOr<Updated>)response!.Value!).IsError.Should().BeTrue();
        }

        [Fact]
        public async Task GivenDeleteAsync_WhenBookDeleted_ShouldReturnNoContent()
        {
            // arrange
            var request = new DeleteBookRequest(Guid.NewGuid());

            _sender.Send(
                Arg.Is<DeleteBookRequest>(x => x.Id == request.Id),
                Arg.Any<CancellationToken>()
            ).Returns(Result.Deleted);

            // act
            var result = await _controller.DeleteAsync(
                request.Id,
                CancellationToken.None
            ) as NoContentResult;

            // assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task GivenDeleteAsync_WhenBookNotFound_ShouldReturnBadRequest()
        {
            // arrange
            var request = new DeleteBookRequest(Guid.NewGuid());

            _sender.Send(
                Arg.Any<DeleteBookRequest>(),
                Arg.Any<CancellationToken>()
            ).Returns(Error.Failure());

            // act
            var result = await _controller.DeleteAsync(
                request.Id,
                CancellationToken.None
            ) as ObjectResult;

            // assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }
    }
}