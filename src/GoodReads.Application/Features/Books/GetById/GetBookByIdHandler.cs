using ErrorOr;

using GoodReads.Domain.BookAggregate.Entities;
using GoodReads.Domain.BookAggregate.ValueObjects;
using GoodReads.Domain.Common.Interfaces.Repositories.EntityFramework;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GoodReads.Application.Features.Books.GetById
{
    public sealed class GetBookByIdHandler :
        IRequestHandler<GetBookByIdRequest, ErrorOr<GetBookByIdResponse>>
    {
        private readonly IRepository<Book, BookId, Guid> _repository;
        private readonly ILogger<GetBookByIdHandler> _logger;

        public GetBookByIdHandler(
            IRepository<Book, BookId, Guid> repository,
            ILogger<GetBookByIdHandler> logger
        )
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ErrorOr<GetBookByIdResponse>> Handle(
            GetBookByIdRequest request,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var book = await _repository.GetByIdAsync(
                BookId.Create(request.Id),
                cancellationToken
            );

            if (book is null)
            {
                return BookNotFoundError(request);
            }

            return new GetBookByIdResponse(
                Title: book.Title,
                Description: book.Description,
                Isbn: book.Isbn,
                Author: book.Author,
                MeanScore: book.MeanScore.Value,
                Gender: book.Gender.Name,
                BookData: new BookDataResponse(
                    Publisher: book.BookData.Publisher,
                    YearOfPublication: book.BookData.YearOfPublication,
                    Pages: book.BookData.Pages
                ),
                Cover: book.Cover.ToArray(),
                RatingIds: book.RatingIds.Select(r => r.Value)
            );
        }

        private Error BookNotFoundError(GetBookByIdRequest request)
        {
            _logger.LogError("Book ({BookId}) was not found", request.Id);

            return Error.NotFound(
                code: "Book.NotFound",
                description: $"Book ({request.Id}) was not found"
            );
        }
    }
}