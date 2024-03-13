using GoodReads.Application.Common.Pagination;

using MongoDB.Driver;

namespace GoodReads.Infrastructure.Mongo.Utils.Facets
{
    public static class DataFacet
    {
        public static IReadOnlyList<TAggregate> GetData<TAggregate>(
            this AggregateFacetResults aggregation
        )
        {
            return aggregation?.Facets?
                .FirstOrDefault(f => f.Name == PaginationConstants.DataFacet)?
                .Output<TAggregate>() ?? new List<TAggregate>();
        }
    }
}