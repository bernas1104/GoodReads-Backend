using System.Linq.Expressions;

using GoodReads.Application.Features.Books.Create;
using GoodReads.Domain.BookAggregate.Entities;
using GoodReads.Domain.BookAggregate.ValueObjects;
using GoodReads.Domain.Common.Interfaces.Repositories.EntityFramework;
using GoodReads.Shared.Mocks;
using GoodReads.Unit.Tests.Helpers;

using Microsoft.Extensions.Logging;

namespace GoodReads.Unit.Tests.Application.Features.Books
{
    public class CreateBookHandlerTest
    {
        private readonly IRepository<Book, BookId, Guid> _repository;
        private readonly ILogger<CreateBookHandler> _logger;
        private readonly CreateBookHandler _handler;

        public CreateBookHandlerTest()
        {
            _repository = Substitute.For<IRepository<Book, BookId, Guid>>();
            _logger = Substitute.For<ILogger<CreateBookHandler>>();
            _handler = new (_repository, _logger);
        }

        [Fact]
        public async Task GivenCreateBookRequest_WhenIsbnAlreadyUsed_ShouldReturnErrorConflict()
        {
            // arrange
            var bookIsbn = "SomeIsbn";
            var request = BookMock.GetCreateBookRequest(bookIsbn);

            _repository.GetByFilterAsync(
                Arg.Any<Expression<Func<Book, bool>>>(),
                Arg.Any<CancellationToken>()
            ).Returns(BookMock.Get(isbn: bookIsbn));

            // act
            var response = await _handler.Handle(request, CancellationToken.None);

            // assert
            response.Should().NotBeNull();
            response!.IsError.Should().BeTrue();

            _logger.ShouldHaveLoggedError($"ISBN {bookIsbn} is already being used");
        }

        [Fact]
        public async Task GivenCreateBookRequest_WhenIsbnNotUsed_ShouldReturnCreatedBookId()
        {
            // arrange
            var bookIsbn = "SomeIsbn";
            var request = BookMock.GetCreateBookRequest(bookIsbn);

            _repository.GetByFilterAsync(
                Arg.Any<Expression<Func<Book, bool>>>(),
                Arg.Any<CancellationToken>()
            ).Returns((Book?)null);

            // act
            var response = await _handler.Handle(request, CancellationToken.None);

            // assert
            response.Should().NotBeNull();
            response!.IsError.Should().BeFalse();
            response!.Value.Should().NotBeEmpty();

            await _repository.Received()
                .AddAsync(
                    Arg.Is<Book>(x => x.Isbn == bookIsbn),
                    Arg.Any<CancellationToken>()
                );

            _logger.ShouldHaveLoggedInformation(
                $"Book {request.Title} was registered successfully"
            );
        }
    }
}