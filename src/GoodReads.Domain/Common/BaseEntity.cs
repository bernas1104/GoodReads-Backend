namespace GoodReads.Domain.Common
{
    public abstract class BaseEntity
    {
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public DateTime? DeletedAt { get; private set; }

        protected BaseEntity()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            DeletedAt = default;
        }

        protected virtual void Update() => UpdatedAt = DateTime.UtcNow;

        public void Delete() => DeletedAt = DateTime.UtcNow;
    }
}