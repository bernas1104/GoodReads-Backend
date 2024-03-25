namespace GoodReads.Domain.Common.MongoDb
{
    public abstract class Entity : BaseEntity
    {
        protected Entity()
        {
        }

        protected Entity(DateTime createdAt) : base(createdAt)
        {
        }
    }
}