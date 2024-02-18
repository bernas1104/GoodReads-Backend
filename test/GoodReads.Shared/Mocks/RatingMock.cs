using System.Diagnostics.CodeAnalysis;

using Bogus;

using GoodReads.Domain.RatingAggregate.Entities;
using GoodReads.Domain.RatingAggregate.ValueObjects;

namespace GoodReads.Shared.Mocks
{
    [ExcludeFromCodeCoverage]
    public static class RatingMock
    {
        public static Rating Get(RatingId? id = null)
        {
            return new Faker<Rating>().CustomInstantiator(f => (
                id is null ?
                    Rating.Create(
                        score: ScoreMock.Get(),
                        description: f.Random.String2(50),
                        reading: ReadingMock.Get(),
                        userId: Guid.NewGuid(),
                        bookId: Guid.NewGuid()
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
    }
}