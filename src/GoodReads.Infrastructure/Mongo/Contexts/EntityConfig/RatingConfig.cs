using GoodReads.Domain.RatingAggregate.Entities;
using GoodReads.Infrastructure.Mongo.Contexts.Interfaces;

using MongoDB.Driver;

namespace GoodReads.Infrastructure.Mongo.Contexts.EntityConfig
{
    public sealed class RatingConfig : EntityConfig<Rating, Guid>
    {
        public RatingConfig(IMongoContext context) : base(context)
        {
            //
        }

        public override void ConfigureIndexes()
        {
            var indexes = new List<CreateIndexModel<Rating>>
            {
                //
            };

            AddIndexes(indexes);
        }
    }
}