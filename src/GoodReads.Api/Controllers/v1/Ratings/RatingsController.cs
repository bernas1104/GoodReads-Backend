using Microsoft.AspNetCore.Mvc;

namespace GoodReads.Api.Controllers.v1.Ratings
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class RatingsController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllAsync() => NoContent();
    }
}