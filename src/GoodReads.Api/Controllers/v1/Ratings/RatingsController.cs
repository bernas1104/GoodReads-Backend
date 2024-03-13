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
            [FromBody] object request,
            CancellationToken cancellationToken
        )
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedAsync(
            [FromQuery] object request,
            CancellationToken cancellationToken
        )
        {
            throw new NotImplementedException();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken
        )
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken
        )
        {
            throw new NotImplementedException();
        }
    }
}