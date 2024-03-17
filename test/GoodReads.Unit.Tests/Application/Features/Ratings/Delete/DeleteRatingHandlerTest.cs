using ErrorOr;

using GoodReads.Application.Common.Repositories.MongoDb;
using GoodReads.Application.Features.Ratings.Delete;
using GoodReads.Domain.Common.Events;
using GoodReads.Domain.RatingAggregate.Entities;
using GoodReads.Domain.RatingAggregate.ValueObjects;
using GoodReads.Shared.Mocks;
using GoodReads.Unit.Tests.Helpers;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GoodReads.Unit.Tests.Application.Features.Ratings.Delete
{
    public class DeleteRatingHandlerTest
    {
        private readonly IRepository<Rating, RatingId, Guid> _repository;
        private readonly IPublisher _publisher;
        private readonly ILogger<DeleteRatingHandler> _logger;
        private readonly DeleteRatingHandler _handler;

        public DeleteRatingHandlerTest()
        {
            _repository = Substitute.For<IRepository<Rating, RatingId, Guid>>();
            _publisher = Substitute.For<IPublisher>();
            _logger = Substitute.For<ILogger<DeleteRatingHandler>>();
            _handler = new (_repository, _publisher, _logger);
        }

        [Fact]
        public async Task GivenDeleteRatingRequest_WhenRatingFound_ShouldDeleteRatingFromRepository()
        {
            // arrange
            var ratingId = Guid.NewGuid();
            var rating = RatingMock.Get(RatingId.Create(ratingId));
            var request = new DeleteRatingRequest(ratingId);

            _repository.GetByIdAsync(
                Arg.Any<RatingId>(),
                Arg.Any<CancellationToken>()
            ).Returns(rating);

            // act
            var response = await _handler.Handle(request, CancellationToken.None);

            // assert
            response.Should().NotBeNull();
            response.IsError.Should().BeFalse();
            response.Value.Should().Be(Result.Deleted);

            await _repository.Received()
                .DeleteAsync(
                    Arg.Is<RatingId>(r => r.Equals(RatingId.Create(ratingId))),
                    Arg.Any<CancellationToken>()
                );

            _logger.ShouldHaveLoggedInformation($"Rating ({ratingId}) was deleted successfully");

            await _publisher.Received()
                .Publish(
                    Arg.Is<RatingDeleted>(r => r.RatingId == ratingId),
                    Arg.Any<CancellationToken>()
                );
        }

        [Fact]
        public async Task GivenDeleteRatingRequest_WhenRatingNotFound_ShouldReturnErrorNotFound()
        {
            // arrange
            var ratingId = Guid.NewGuid();
            var request = new DeleteRatingRequest(ratingId);

            _repository.GetByIdAsync(
                Arg.Any<RatingId>(),
                Arg.Any<CancellationToken>()
            ).Returns((Rating?)null);

            // act
            var response = await _handler.Handle(request, CancellationToken.None);

            // assert
            response.Should().NotBeNull();
            response.IsError.Should().BeTrue();
            response.FirstError.Type.Should().Be(ErrorType.NotFound);

            await _repository.DidNotReceive()
                .DeleteAsync(
                    Arg.Is<RatingId>(r => r.Equals(RatingId.Create(ratingId))),
                    Arg.Any<CancellationToken>()
                );

            _logger.ShouldHaveLoggedWarning($"Rating ({ratingId}) was not found");

            await _publisher.DidNotReceive()
                .Publish(
                    Arg.Is<RatingDeleted>(r => r.RatingId == ratingId),
                    Arg.Any<CancellationToken>()
                );
        }
    }
}