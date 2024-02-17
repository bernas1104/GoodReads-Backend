using GoodReads.Domain.Common;
using GoodReads.Domain.UserAggregate.ValueObjects;

namespace GoodReads.Domain.UserAggregate.Entities
{
    public sealed class User : AggregateRoot<UserId, Guid>
    {
        public string Name { get; private set; }
        public string Email { get; private set; }
        public IReadOnlyList<Guid> Ratings { get => _ratings.AsReadOnly(); }
        private readonly List<Guid> _ratings;

        public User(string name, string email) : base(UserId.CreateUnique())
        {
            Name = name;
            Email = email;

            _ratings = new List<Guid>();
        }

        public void AddRating(Guid ratingId)
        {
            _ratings.Add(ratingId);
        }
    }
}