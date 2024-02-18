namespace GoodReads.Infrastructure.Mongo.Utils
{
    public sealed class MongoConnectionOptions
    {
        public const string Mongo = "Mongo";
        public string? ConnectionString { get; init; }
        public string? Schema { get; init; }
        public int LogTtlDays { get; init; }
    }
}