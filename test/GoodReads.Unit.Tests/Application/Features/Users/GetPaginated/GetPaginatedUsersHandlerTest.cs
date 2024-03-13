using GoodReads.Application.Features.Users.GetPaginated;
using GoodReads.Application.Common.Repositories.EntityFramework;
using GoodReads.Domain.UserAggregate.Entities;
using GoodReads.Domain.UserAggregate.ValueObjects;
using GoodReads.Shared.Mocks;
using System.Linq.Expressions;

namespace GoodReads.Unit.Tests.Application.Features.Users.GetPaginated
{
    public class GetPaginatedUsersHandlerTest
    {
        private readonly IRepository<User, UserId, Guid> _repository;
        private readonly GetPaginatedUsersHandler _handler;

        public GetPaginatedUsersHandlerTest()
        {
            _repository = Substitute.For<IRepository<User, UserId, Guid>>();
            _handler = new (_repository);
        }

        [Fact]
        public async Task GivenGetPaginatedUsersRequest_ShouldReturnPaginatedUsers()
        {
            // arrange
            var count = new Faker().Random.Int(1, 10);
            var request = UserMock.GetPaginatedUsersRequest();

            _repository.GetPaginatedAsync(
                Arg.Any<Expression<Func<User, bool>>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<CancellationToken>()
            ).Returns(new List<User>{ UserMock.Get() });

            _repository.GetCountAsync(Arg.Any<CancellationToken>())
                .Returns(count);

            // act
            var response = await _handler.Handle(request, CancellationToken.None);

            // assert
            response.Should().NotBeNull();
            response.Data.Should().HaveCount(1);
            response.CurrentPage.Should().Be(request.Page);
            response.TotalItens.Should().Be(count);
            response.TotalPages.Should().Be((int)Math.Ceiling(count / (decimal)request.Size));
            response.PageSize.Should().Be(request.Size);
        }
    }
}