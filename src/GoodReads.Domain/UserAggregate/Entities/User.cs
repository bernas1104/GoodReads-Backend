using GoodReads.Domain.Common.EntityFramework;
using GoodReads.Domain.UserAggregate.ValueObjects;

namespace GoodReads.Domain.UserAggregate.Entities
{
    public sealed class User : AggregateRoot<UserId, Guid>
    {
        public string Name { get; private set; }
        public string Email { get; private set; }
        public IReadOnlyList<RatingId> RatingIds { get => _ratingIds.AsReadOnly(); }
        private readonly List<RatingId> _ratingIds;

        public User(string name, string email)
        {
            Id = UserId.CreateUnique();

            Name = name;
            Email = email;

            _ratingIds = new List<RatingId>();
        }

        public User(UserId id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;

            _ratingIds = new List<RatingId>();
        }

        public void AddRating(RatingId ratingId)
        {
            _ratingIds.Add(ratingId);
        }

        public void Update(string name, string email)
        {
            Name = name;
            Email = email;
            Update();
        }
    }
}