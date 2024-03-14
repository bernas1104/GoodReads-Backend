using ErrorOr;

using GoodReads.Application.Features.Users.Create;
using GoodReads.Application.Features.Users.Delete;
using GoodReads.Application.Features.Users.GetById;
using GoodReads.Application.Features.Users.GetPaginated;
using GoodReads.Application.Features.Users.Update;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace GoodReads.Api.Controllers.v1.Users
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public sealed class UsersController : ControllerBase
    {
        private readonly ISender _sender;

        public UsersController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(
            [FromBody] CreateUserRequest request,
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
            [FromQuery] int page,
            [FromQuery] int size,
            CancellationToken cancellationToken
        )
        {
            var result = await _sender.Send(
                new GetPaginatedUsersRequest(page, size),
                cancellationToken
            );

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken
        )
        {
            var result = await _sender.Send(
                new GetUserByIdRequest(id),
                cancellationToken
            );

            return Ok(result);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateUserRequest request,
            CancellationToken cancellationToken
        )
        {
            if (id != request.Id)
            {
                ErrorOr<Updated> error = Error.Validation(
                    code: "User.Validation",
                    description: "Route Id must be equal to body Id"
                );

                return BadRequest(error);
            }

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
                new DeleteUserRequest(id),
                cancellationToken
            );

            return result.IsError ?
                BadRequest(result) :
                NoContent();
        }
    }
}