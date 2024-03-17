using System.Linq.Expressions;

using ErrorOr;

using GoodReads.Application.Common.Repositories.MongoDb;
using GoodReads.Application.Features.Ratings.Update;
using GoodReads.Domain.RatingAggregate.Entities;
using GoodReads.Domain.RatingAggregate.ValueObjects;
using GoodReads.Shared.Mocks;
using GoodReads.Unit.Tests.Helpers;

using Microsoft.Extensions.Logging;

namespace GoodReads.Unit.Tests.Application.Features.Ratings.Update
{
    public class UpdateRatingHandlerTest
    {
        private readonly IRepository<Rating, RatingId, Guid> _repository;
        private readonly ILogger<UpdateRatingHandler> _logger;
        private readonly UpdateRatingHandler _handler;

        public UpdateRatingHandlerTest()
        {
            _repository = Substitute.For<IRepository<Rating, RatingId, Guid>>();
            _logger = Substitute.For<ILogger<UpdateRatingHandler>>();
            _handler = new (_repository, _logger);
        }

        [Fact]
        public async Task GivenUpdateRatingRequest_WhenRatingNotFound_ShouldReturnErrorNotFound()
        {
            // arrange
            var ratingId = Guid.NewGuid();
            var request = new UpdateRatingRequest(
                ratingId,
                "Some description"
            );

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
                .UpdateAsync(
                    Arg.Any<Expression<Func<Rating, bool>>>(),
                    Arg.Any<Rating>(),
                    Arg.Any<CancellationToken>()
                );

            _logger.ShouldHaveLoggedError($"Rating ({ratingId}) was not found");
        }

        [Fact]
        public async Task GivenUpdateRatingRequest_WhenRatingFound_ShouldReturnUpdated()
        {
            // arrange
            var ratingId = Guid.NewGuid();
            var request = new UpdateRatingRequest(
                ratingId,
                "Some description"
            );

            _repository.GetByIdAsync(
                Arg.Is<RatingId>(r => r.Equals(RatingId.Create(ratingId))),
                Arg.Any<CancellationToken>()
            ).Returns(RatingMock.Get(RatingId.Create(ratingId)));

            // act
            var response = await _handler.Handle(request, CancellationToken.None);

            // assert
            response.Should().NotBeNull();
            response.IsError.Should().BeFalse();
            response.Value.Should().BeOfType<Updated>();

            await _repository.Received()
                .UpdateAsync(
                    Arg.Any<Expression<Func<Rating, bool>>>(),
                    Arg.Is<Rating>(r => r.Id.Equals(RatingId.Create(ratingId))),
                    Arg.Any<CancellationToken>()
                );

            _logger.ShouldHaveLoggedInformation(
                $"Rating ({ratingId}) was updated successfully"
            );
        }
    }
}