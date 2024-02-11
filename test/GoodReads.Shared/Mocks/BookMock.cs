using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Bogus;

using GoodReads.Domain.BookAggregate.Entities;
using GoodReads.Domain.BookAggregate.Enums;

namespace GoodReads.Shared.Mocks
{
    public static class BookMock
    {
        public static Book Get(
            string? title = null,
            string? isbn = null,
            string? author = null,
            Gender? gender = null
        )
        {
            return new Faker<Book>().CustomInstantiator(f =>
                Book.Create(
                    title: title ?? f.Random.String2(10),
                    isbn: isbn ?? f.Random.String2(20),
                    author: author ?? f.Person.FullName,
                    gender: gender ?? Gender.FromValue(f.Random.Int(0, 5))
                )
            );
        }
    }
}