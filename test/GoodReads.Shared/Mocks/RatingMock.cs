using System.Diagnostics.CodeAnalysis;

using Bogus;

using GoodReads.Application.Features.Ratings;
using GoodReads.Application.Features.Ratings.GetPaginated;
using GoodReads.Domain.Common.Events;
using GoodReads.Domain.RatingAggregate.Entities;
using GoodReads.Domain.RatingAggregate.ValueObjects;

namespace GoodReads.Shared.Mocks
{
    [ExcludeFromCodeCoverage]
    public static class RatingMock
    {
        public static Rating Get(
            RatingId? id = null,
            Guid ? userId = null,
            Guid ? bookId = null
        )
        {
            return new Faker<Rating>().CustomInstantiator(f => (
                id is null ?
                    Rating.Create(
                        score: ScoreMock.Get(),
                        description: f.Random.String2(50),
                        reading: ReadingMock.Get(),
                        userId: userId ?? Guid.NewGuid(),
                        bookId: bookId ?? Guid.NewGuid()
                    ) :
                    Rating.Instantiate(
                        id!,
                        score: ScoreMock.Get(),
                        description: f.Random.String2(10),
                        reading: ReadingMock.Get(),
                        userId: Guid.NewGuid(),
                        bookId: Guid.NewGuid()
                    )
            ));
        }

        public static CreateRatingRequest GetCreateRatingRequest()
        {
            return new Faker<CreateRatingRequest>().CustomInstantiator(f => (
                new CreateRatingRequest(
                    Score: f.Random.Int(1, 5),
                    Description: f.Random.String2(20),
                    Reading: new CreateReadingRequest(
                        InitiatedAt: f.Date.Recent(),
                        FinishedAt: f.Date.Recent().AddDays(14)
                    ),
                    UserId: Guid.NewGuid(),
                    BookId: Guid.NewGuid()
                )
            ));
        }

        public static GetPaginatedRatingsRequest GetPaginatedRatingsRequest(
            UsedId usedId = UsedId.Book,
            Guid? bookId = null,
            Guid? userId = null,
            int? onlyScoresOf = null
        )
        {
            return new Faker<GetPaginatedRatingsRequest>().CustomInstantiator(f => (
                new GetPaginatedRatingsRequest(
                    Page: f.Random.Int(1, 5),
                    Size: f.Random.Int(10, 20),
                    BookId: usedId == UsedId.Book ?
                        (bookId ?? Guid.NewGuid()) : null,
                    UserId: usedId == UsedId.User ?
                        (userId ?? Guid.NewGuid()) : null,
                    OnlyScoresOf: onlyScoresOf ?? f.Random.Int(1, 5)
                )
            ));
        }

        public static Faker<RatingResponse> GetFakeRatingResponse()
        {
            return new Faker<RatingResponse>().CustomInstantiator(f => (
                new RatingResponse(
                    Score: f.Random.Int(1, 5),
                    Description: f.Random.String2(20),
                    Reading: new ReadingResponse(
                        InitiatedAt: f.Date.Recent(),
                        FinishedAt: f.Date.Recent().AddDays(14)
                    ),
                    CreatedAt: DateTime.UtcNow
                )
            ));
        }

        public static RatingCreated GetRatingCreatedEvent(
            Guid? ratingId = null,
            Guid? userId = null,
            Guid? bookId = null
        )
        {
            return new Faker<RatingCreated>().CustomInstantiator(f => (
                RatingCreated.Create(
                    Get(
                        id: RatingId.Create(ratingId ?? Guid.NewGuid()),
                        userId: userId ?? Guid.NewGuid(),
                        bookId: bookId ?? Guid.NewGuid()
                    )
                )
            ));
        }
    }

    public enum UsedId
    {
        Book,
        User
    }
}