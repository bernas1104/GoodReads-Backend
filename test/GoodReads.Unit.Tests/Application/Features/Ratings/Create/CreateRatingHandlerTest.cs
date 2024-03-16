using System.Linq.Expressions;

using GoodReads.Application.Common.Repositories.MongoDb;
using GoodReads.Application.Features.Ratings.Create;
using GoodReads.Domain.Common.Events;
using GoodReads.Domain.RatingAggregate.Entities;
using GoodReads.Domain.RatingAggregate.ValueObjects;
using GoodReads.Shared.Mocks;
using GoodReads.Unit.Tests.Helpers;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GoodReads.Unit.Tests.Application.Features.Ratings.Create
{
    public class CreateRatingHandlerTest
    {
        private readonly IRepository<Rating, RatingId, Guid> _repository;
        private readonly IPublisher _publisher;
        private readonly ILogger<CreateRatingHandler> _logger;
        private readonly CreateRatingHandler _handler;

        public CreateRatingHandlerTest()
        {
            _repository = Substitute.For<IRepository<Rating, RatingId, Guid>>();
            _publisher = Substitute.For<IPublisher>();
            _logger = Substitute.For<ILogger<CreateRatingHandler>>();
            _handler = new (_repository, _publisher, _logger);
        }

        [Fact]
        public async Task GivenCreateRatingHandler_WhenUserDidNotPostRatingToBook_ShouldReturnCreatedRatingId()
        {
            // arrange
            var request = RatingMock.GetCreateRatingRequest();
            var bookId = request.BookId;
            var userId = request.UserId;

            _repository.GetByFilterAsync(
                Arg.Any<Expression<Func<Rating, bool>>>(),
                Arg.Any<CancellationToken>()
            ).Returns((Rating?)null);

            // act
            var response = await _handler.Handle(request, CancellationToken.None);

            // assert
            response.Should().NotBeNull();
            response.IsError.Should().BeFalse();

            await _repository.Received()
                .AddAsync(
                    Arg.Is<Rating>(
                        r => r.BookId.Equals(BookId.Create(bookId)) &&
                            r.UserId.Equals(UserId.Create(userId))
                    ),
                    Arg.Any<CancellationToken>()
                );

            await _publisher.Received()
                .Publish(
                    Arg.Is<RatingCreated>(
                        e => e.BookId == bookId && e.UserId == userId
                    ),
                    Arg.Any<CancellationToken>()
                );

            _logger.ShouldHaveLoggedInformation(
                $"User ({userId}) rating for book ({bookId}) was created successfully"
            );
        }

        [Fact]
        public async Task GivenCreateRatingHandler_WhenUserDidPostRatingToBook_ShouldReturnError()
        {
            // arrange
            var request = RatingMock.GetCreateRatingRequest();
            var bookId = request.BookId;
            var userId = request.UserId;
            var rating = RatingMock.Get(userId: userId, bookId: bookId);

            _repository.GetByFilterAsync(
                Arg.Any<Expression<Func<Rating, bool>>>(),
                Arg.Any<CancellationToken>()
            ).Returns(rating);

            // act
            var response = await _handler.Handle(request, CancellationToken.None);

            // assert
            response.Should().NotBeNull();
            response.IsError.Should().BeTrue();

            await _repository.DidNotReceive()
                .AddAsync(
                    Arg.Any<Rating>(),
                    Arg.Any<CancellationToken>()
                );

            _logger.ShouldHaveLoggedError(
                $"User ({userId}) already posted a rating for book ({bookId})"
            );
        }
    }
}