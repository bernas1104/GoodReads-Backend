using GoodReads.Domain.Common.Interfaces.Repositories.EntityFramework;
using GoodReads.Domain.UserAggregate.Entities;
using GoodReads.Domain.UserAggregate.ValueObjects;

using Microsoft.AspNetCore.Mvc;

namespace GoodReads.Api.Controllers.v1.Users
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IRepository<User, UserId, Guid> _repository;

        public UsersController(IRepository<User, UserId, Guid> repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> GetAllAsync(
            [FromBody] UserRequest request,
            CancellationToken cancellationToken
        )
        {
            var user = new User(request.Name, request.Email);
            await _repository.AddAsync(user, cancellationToken);

            return Ok(user);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken
        )
        {
            var user = await _repository.GetByIdAsync(UserId.Create(id), cancellationToken);
            return Ok(user);
        }
    }

    public record UserRequest(string Name, string Email);
}