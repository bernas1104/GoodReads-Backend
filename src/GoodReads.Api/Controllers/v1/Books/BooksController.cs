using GoodReads.Application.Features.Books.Create;
using GoodReads.Application.Features.Books.Delete;
using GoodReads.Application.Features.Books.GetById;
using GoodReads.Application.Features.Books.GetPaginated;
using GoodReads.Application.Features.Books.Update;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace GoodReads.Api.Controllers.v1.Books
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class BooksController : ControllerBase
    {
        private readonly ISender _sender;

        public BooksController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(
            [FromBody] CreateBookRequest request,
            CancellationToken cancellationToken
        )
        {
            var result = await _sender.Send(request, cancellationToken);

            return CreatedAtAction(
                nameof(GetByIdAsync),
                new { id = result.Value },
                result
            );
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedAsync(
            [FromQuery] GetPaginatedBooksRequest request,
            CancellationToken cancellationToken
        )
        {
            var result = await _sender.Send(request, cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken
        )
        {
            var result = await _sender.Send(
                new GetBookByIdRequest(id),
                cancellationToken
            );

            return Ok(result);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateBookRequest request,
            CancellationToken cancellationToken
        )
        {
            request = new UpdateBookRequest(id, request.Description, request.Cover);

            var result = await _sender.Send(request, cancellationToken);

            return result.IsError ?
                BadRequest(result) :
                NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken
        )
        {
            var result = await _sender.Send(
                new DeleteBookRequest(id),
                cancellationToken
            );

            return result.IsError ?
                BadRequest(result) :
                NoContent();
        }
    }
}