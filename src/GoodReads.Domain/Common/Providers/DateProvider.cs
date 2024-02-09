using GoodReads.Domain.Common.Interfaces.Providers;

namespace GoodReads.Domain.Common.Providers
{
    public class DateProvider : IDateProvider
    {
        public int GetCurrentYear()
        {
            return DateTime.UtcNow.Year;
        }
    }
}