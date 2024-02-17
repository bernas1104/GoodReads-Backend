using System.Diagnostics.CodeAnalysis;

using Bogus;

using GoodReads.Domain.RatingAggregate.ValueObjects;

namespace GoodReads.Shared.Mocks
{
    [ExcludeFromCodeCoverage]
    public static class ReadingMock
    {
        public static Reading Get(
            DateTime? initiatedAt = null,
            DateTime? finishedAt = null
        )
        {
            return new Faker<Reading>()
                .CustomInstantiator(f => {
                    var initiated = initiatedAt ?? f.Date.Recent();
                    var finished = finishedAt ??
                        initiated.AddDays(f.Random.Int(1, 10));

                    return new Reading(
                        initiatedAt: initiated,
                        finishedAt: finished
                    );
                });
        }
    }
}