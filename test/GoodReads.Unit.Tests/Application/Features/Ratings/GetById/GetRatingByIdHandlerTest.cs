using ErrorOr;

using GoodReads.Application.Common.Repositories.MongoDb;
using GoodReads.Application.Features.Ratings;
using GoodReads.Application.Features.Ratings.GetById;
using GoodReads.Domain.RatingAggregate.Entities;
using GoodReads.Domain.RatingAggregate.ValueObjects;
using GoodReads.Shared.Mocks;
using GoodReads.Unit.Tests.Helpers;

using Microsoft.Extensions.Logging;

namespace GoodReads.Unit.Tests.Application.Features.Ratings.GetById
{
    public class GetRatingByIdHandlerTest
    {
        private readonly IRepository<Rating, RatingId, Guid> _repository;
        private readonly ILogger<GetRatingByIdHandler> _logger;
        private readonly GetRatingByIdHandler _handler;

        public GetRatingByIdHandlerTest()
        {
            _repository = Substitute.For<IRepository<Rating, RatingId, Guid>>();
            _logger = Substitute.For<ILogger<GetRatingByIdHandler>>();
            _handler = new (_repository, _logger);
        }

        [Fact]
        public async Task GivenGetRatingByIdRequest_WhenRatingFound_ShouldReturnRatingResponse()
        {
            // arrange
            var ratingId = Guid.NewGuid();
            var request = new GetRatingByIdRequest(ratingId);

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

            _logger.ShouldHaveLoggedError($"Rating ({ratingId}) was not found");
        }

        [Fact]
        public async Task GivenGetRatingByIdRequest_WhenRatingNotFound_ShouldReturnErrorNotFound()
        {
            // arrange
            var ratingId = Guid.NewGuid();
            var rating = RatingMock.Get(id: RatingId.Create(ratingId));
            var request = new GetRatingByIdRequest(ratingId);

            _repository.GetByIdAsync(
                Arg.Is<RatingId>(r => r.Equals(RatingId.Create(ratingId))),
                Arg.Any<CancellationToken>()
            ).Returns(rating);

            // act
            var response = await _handler.Handle(request, CancellationToken.None);

            // assert
            response.Should().NotBeNull();
            response.IsError.Should().BeFalse();
            response.Value.Should().BeOfType<RatingResponse>();
        }
    }
}