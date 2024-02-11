using ErrorOr;

using GoodReads.Domain.Common;

namespace GoodReads.Domain.UserAggregate.Entities
{
    public class User : AggregateRoot
    {
        public string Name { get; private set; }
        public string Email { get; private set; }
        public IReadOnlyList<Guid> Ratings { get => _ratings.ToList(); }
        private readonly List<Guid> _ratings;

        public User(string name, string email)
        {
            Name = name;
            Email = email;

            _ratings = new List<Guid>();
        }

        public ErrorOr<Success> AddRating(Guid ratingId)
        {
            _ratings.Add(ratingId);

            return Result.Success;
        }
    }
}