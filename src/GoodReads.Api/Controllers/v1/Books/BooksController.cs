using Microsoft.AspNetCore.Mvc;

namespace GoodReads.Api.Controllers.v1.Books
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class BooksController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllAsync() => NoContent();
    }
}