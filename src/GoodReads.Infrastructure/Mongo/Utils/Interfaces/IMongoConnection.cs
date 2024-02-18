using MongoDB.Driver;

namespace GoodReads.Infrastructure.Mongo.Utils.Interfaces
{
    public interface IMongoConnection
    {
        IMongoDatabase GetDatabase();
    }
}