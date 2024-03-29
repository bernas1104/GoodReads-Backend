using System.Diagnostics.CodeAnalysis;

using Bogus;

using GoodReads.Application.Features.Users;
using GoodReads.Application.Features.Users.Create;
using GoodReads.Application.Features.Users.GetById;
using GoodReads.Application.Features.Users.GetPaginated;
using GoodReads.Application.Features.Users.Update;
using GoodReads.Domain.Common.Events;
using GoodReads.Domain.UserAggregate.Entities;
using GoodReads.Domain.UserAggregate.ValueObjects;

namespace GoodReads.Shared.Mocks
{
    [ExcludeFromCodeCoverage]
    public static class UserMock
    {
        public static User Get(UserId? id = null, string? email = null)
        {
            return new Faker<User>().CustomInstantiator(f => (
                id is null ?
                    new User(
                        name: f.Person.FullName,
                        email: email ?? f.Internet.Email()
                    ) :
                    new User(
                        id: id,
                        name: f.Person.FullName,
                        email: email ?? f.Internet.Email()
                    )
            ));
        }

        public static CreateUserRequest GetCreateUserRequest(
            string? name = null,
            string? email = null
        ) => new Faker<CreateUserRequest>().CustomInstantiator(f => (
            new CreateUserRequest(
                Name: name ?? f.Person.FullName,
                Email: email ?? f.Internet.Email()
            )
        ));

        public static GetPaginatedUsersRequest GetPaginatedUsersRequest(
            int? page = null,
            int? size = null
        ) => new Faker<GetPaginatedUsersRequest>().CustomInstantiator(f => (
            new GetPaginatedUsersRequest(
                Page: page ?? f.Random.Int(1, 10),
                Size: size ?? f.Random.Int(5, 10)
            )
        ));

        public static Faker<UserResponse> GetUserResponse(
            string? name = null,
            int? totalRatings = null
        )
        {
            return new Faker<UserResponse>().CustomInstantiator(f => (
                new UserResponse(
                    Name: name ?? f.Person.FullName,
                    TotalRatings: totalRatings ?? f.Random.Int(0, 100)
                )
            ));
        }

        public static GetUserByIdResponse GetUserByIdResponse(
            string? name = null,
            string? email = null,
            int? totalRatings = null
        ) =>
            new Faker<GetUserByIdResponse>().CustomInstantiator(f => (
                new GetUserByIdResponse(
                    Name: name ?? f.Person.FullName,
                    Email: email ?? f.Internet.Email(),
                    TotalRatings: totalRatings ?? f.Random.Int(0, 100),
                    Ratings: new UserRatingIdsResponse(new List<Guid> { Guid.NewGuid() }),
                    CreatedAt: DateTime.UtcNow
                )
            ));

        public static UpdateUserRequest GetUpdateUserRequest(
            Guid? id = null,
            string? name = null,
            string? email = null
        ) => new Faker<UpdateUserRequest>().CustomInstantiator(f => (
            new UpdateUserRequest(
                Id: id ?? Guid.NewGuid(),
                Name: name ?? f.Person.FullName,
                Email: email ?? f.Internet.Email()
            )
        ));

        public static UserRatingAdded GetUserRatingAdded(
            Guid? ratingId = null,
            Guid? userId = null,
            Guid? bookId = null
        )
        {
            return new Faker<UserRatingAdded>().CustomInstantiator(f => (
                UserRatingAdded.Create(
                    RatingMock.GetRatingCreatedEvent(
                        ratingId ?? Guid.NewGuid(),
                        userId ?? Guid.NewGuid(),
                        bookId ?? Guid.NewGuid()
                    )
                )
            ));
        }
    }
}