using GoodReads.Application.Common.Pagination;

using MongoDB.Driver;

namespace GoodReads.Infrastructure.Mongo.Utils.Facets
{
    public static class CountFacet
    {
        public static long GetCount(this AggregateFacetResults? aggregation)
        {
            return aggregation?.Facets?.FirstOrDefault(
                f => f.Name == PaginationConstants.CountFacet
            )?
            .Output<AggregateCountResult>()?
            .FirstOrDefault()?.Count ?? 0;
        }
    }
}