using ErrorOr;

using GoodReads.Application.Features.Ratings;
using GoodReads.Application.Features.Ratings.Delete;
using GoodReads.Application.Features.Ratings.GetById;
using GoodReads.Application.Features.Ratings.GetPaginated;
using GoodReads.Application.Features.Ratings.Update;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace GoodReads.Api.Controllers.v1.Ratings
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public sealed class RatingsController : ControllerBase
    {
        private readonly ISender _sender;

        public RatingsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(
            [FromBody] CreateRatingRequest request,
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
            [FromQuery] GetPaginatedRatingsRequest request,
            CancellationToken cancellationToken
        )
        {
            var response = await _sender.Send(request, cancellationToken);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken
        )
        {
            var response = await _sender.Send(
                new GetRatingByIdRequest(id),
                cancellationToken
            );

            return Ok(response);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateRatingRequest request,
            CancellationToken cancellationToken
        )
        {
            if (id != request.Id)
            {
                ErrorOr<Updated> error = Error.Validation(
                    code: "Rating.Validation",
                    description: "Route Id must be equal to body Id"
                );

                return BadRequest(error);
            }

            var response = await _sender.Send(request, cancellationToken);

            return response.IsError ?
                BadRequest(response) :
                NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken
        )
        {
            var response = await _sender.Send(
                new DeleteRatingRequest(id),
                cancellationToken
            );

            return response.IsError ?
                BadRequest(response) :
                NoContent();
        }
    }
}