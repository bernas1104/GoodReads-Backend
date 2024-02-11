using GoodReads.Domain.Common;

namespace GoodReads.Domain.BookAggregate.ValueObjects
{
    public sealed class MeanScore : ValueObject
    {
        public decimal Value {
            get {
                var sum = 0m;
                var totalRatings = 0;

                foreach (var score in _scores)
                {
                    sum += score.Key * score.Value;
                    totalRatings += score.Value;
                }

                return totalRatings != 0 ?
                    sum / totalRatings :
                    default;
            }
        }
        private readonly IDictionary<int, int> _scores = new Dictionary<int, int>
        {
            { 1, 0 },
            { 2, 0 },
            { 3, 0 },
            { 4, 0 },
            { 5, 0 }
        };

        public void Update(int score)
        {
            _scores[score] += 1;
        }

        public override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}