using GoodReads.Domain.Common.Interfaces.Providers;

namespace GoodReads.Shared.Providers
{
    public class FakeDateProvider : IDateProvider
    {
        private readonly DateTime _dateTime;

        public FakeDateProvider(DateTime dateTime)
        {
            _dateTime = dateTime;
        }

        public int GetCurrentYear()
        {
            return _dateTime.Year;
        }
    }
}