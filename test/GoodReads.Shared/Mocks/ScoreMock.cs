using Bogus;

using GoodReads.Domain.BookAggregate.ValueObjects;

namespace GoodReads.Shared.Mocks
{
    public static class ScoreMock
    {
        public static Score Get(int? value = null)
        {
            return new Faker<Score>()
                .CustomInstantiator(f => (
                    new Score(value ?? f.Random.Int(1, 5))
                ));
        }
    }
}