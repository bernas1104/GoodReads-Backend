using MongoDB.Driver;

namespace GoodReads.Infrastructure.Mongo.Contexts.Interfaces
{
    public interface IMongoContext
    {
        int LogTtlDays { get; }
        IMongoDatabase GetDatabase();
        IMongoCollection<T> GetCollection<T>(string? name = null);
    }
}