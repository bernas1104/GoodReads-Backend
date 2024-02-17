using Microsoft.AspNetCore.Mvc;

namespace GoodReads.Api.Controllers.v1.Users
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllAsync() => NoContent();
    }
}