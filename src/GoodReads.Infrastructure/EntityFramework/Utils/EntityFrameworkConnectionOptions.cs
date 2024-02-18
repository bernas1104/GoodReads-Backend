namespace GoodReads.Infrastructure.EntityFramework.Utils
{
    public class EntityFrameworkConnectionOptions
    {
        public const string EntityFramework = "EntityFramework";
        public string? ConnectionString { get; init; }
        public string? Schema { get; init; }
    }
}