using GoodReads.Domain.RatingAggregate.Entities;
using GoodReads.Domain.RatingAggregate.ValueObjects;
using GoodReads.Infrastructure.Mongo.Contexts.Interfaces;

using MongoDB.Driver;

namespace GoodReads.Infrastructure.Mongo.Contexts.AggregateConfig
{
    public sealed class RatingConfig : AggregateConfig<Rating, RatingId, Guid>
    {
        public RatingConfig(IMongoContext context) : base(context)
        {
            //
        }

        public override void ConfigureIndexes()
        {
            var indexes = new List<CreateIndexModel<Rating>>
            {
                new CreateIndexModel<Rating>(
                    Builder.Ascending(x => x.CreatedAt)
                ),
                new CreateIndexModel<Rating>(
                    Builder.Ascending(x => x.BookId)
                ),
                new CreateIndexModel<Rating>(
                    Builder.Ascending(x => x.UserId)
                ),
                new CreateIndexModel<Rating>(
                    Builder.Ascending(x => x.BookId)
                        .Descending(x => x.UserId)
                ),
                new CreateIndexModel<Rating>(
                    Builder.Ascending(x => x.Score.Value)
                )
            };

            AddIndexes(indexes);
        }
    }
}