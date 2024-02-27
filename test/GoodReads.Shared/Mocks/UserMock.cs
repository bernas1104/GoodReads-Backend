using System.Diagnostics.CodeAnalysis;

using Bogus;

using GoodReads.Domain.UserAggregate.Entities;
using GoodReads.Domain.UserAggregate.ValueObjects;

namespace GoodReads.Shared.Mocks
{
    [ExcludeFromCodeCoverage]
    public static class UserMock
    {
        public static User Get(UserId? id = null)
        {
            return new Faker<User>().CustomInstantiator(f => (
                id is null ?
                    new User(
                        name: f.Person.FullName,
                        email: f.Internet.Email()
                    ) :
                    new User(
                        id: id,
                        name: f.Person.FullName,
                        email: f.Internet.Email()
                    )
            ));
        }
    }
}