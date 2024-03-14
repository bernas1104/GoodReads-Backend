using System.Diagnostics.CodeAnalysis;

using Bogus;

using GoodReads.Application.Common.Pagination;

namespace GoodReads.Shared.Mocks
{
    [ExcludeFromCodeCoverage]
    public static class PaginationMock
    {
        public static PaginatedResponse<TType> GetPaginatedResponse<TType>(
            IEnumerable<TType> data
        )
        {
            return new Faker<PaginatedResponse<TType>>().CustomInstantiator(f => (
                new PaginatedResponse<TType>(
                    Data: data,
                    CurrentPage: f.Random.Int(1, 5),
                    TotalItens: f.Random.Int(10, 20),
                    TotalPages: f.Random.Int(1, 5),
                    PageSize: f.Random.Int(5, 10)
                )
            ));
        }
    }
}