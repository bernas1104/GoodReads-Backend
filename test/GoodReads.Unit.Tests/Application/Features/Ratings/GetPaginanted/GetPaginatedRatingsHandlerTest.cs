using System.Linq.Expressions;

using GoodReads.Application.Common.Pagination;
using GoodReads.Application.Common.Repositories.MongoDb;
using GoodReads.Application.Features.Ratings.GetPaginated;
using GoodReads.Domain.RatingAggregate.Entities;
using GoodReads.Domain.RatingAggregate.ValueObjects;
using GoodReads.Shared.Mocks;

namespace GoodReads.Unit.Tests.Application.Features.Ratings.GetPaginanted
{
    public class GetPaginatedRatingsHandlerTest
    {
        private readonly IRepository<Rating, RatingId, Guid> _repository;
        private readonly GetPaginatedRatingsHandler _handler;

        public GetPaginatedRatingsHandlerTest()
        {
            _repository = Substitute.For<IRepository<Rating, RatingId, Guid>>();
            _handler = new (_repository);
        }

        [Theory]
        [InlineData(null, "9F78E55C-749E-4E62-8A5C-85B689AAE4A3", null)]
        [InlineData("9F78E55C-749E-4E62-8A5C-85B689AAE4A3", null, 5)]
        public async void GivenGetPaginatedRatingsRequest_ShouldReturnPaginatedRatings(
            string? bookId,
            string? userId,
            int? onlyScoresOf
        )
        {
            // arrange
            Guid? parsedBookId = null;
            TryParseGuid(ref parsedBookId, bookId);

            Guid? parsedUserId = null;
            TryParseGuid(ref parsedBookId, userId);

            var request = new GetPaginatedRatingsRequest(
                PaginationConstants.DefaultPage,
                PaginationConstants.DefaultPageSize,
                parsedBookId,
                parsedUserId,
                onlyScoresOf
            );

            _repository.GetPaginatedAsync(
                Arg.Any<Expression<Func<Rating, bool>>?>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<CancellationToken>()
            ).Returns(new List<Rating> { RatingMock.Get() });

            _repository.GetCountAsync(
                Arg.Any<Expression<Func<Rating, bool>>?>(),
                Arg.Any<CancellationToken>()
            ).Returns(1);

            // act
            var response = await _handler.Handle(request, CancellationToken.None);

            // assert
            response.Should().NotBeNull();
            response.Data.Should().HaveCount(1);
        }

        private static void TryParseGuid(ref Guid? parsedGuid, string? guidString)
        {
            if (Guid.TryParse(guidString, out var bookParsed))
            {
                parsedGuid = bookParsed;
            }
        }
    }
}