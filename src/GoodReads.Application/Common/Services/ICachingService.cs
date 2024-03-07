namespace GoodReads.Application.Common.Services
{
    public interface ICachingService
    {
        Task<TValue?> GetAsync<TValue>(
            string key,
            CancellationToken cancellationToken
        );
        Task SetAsync<TValue>(
            string key,
            TValue value,
            DateTimeOffset? expirationDate,
            CancellationToken cancellationToken
        );
    }
}