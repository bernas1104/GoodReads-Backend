using GoodReads.Domain.Common.Interfaces.Repositories;
using GoodReads.Domain.RatingAggregate.Entities;
using GoodReads.Domain.RatingAggregate.ValueObjects;

using Microsoft.AspNetCore.Mvc;

namespace GoodReads.Api.Controllers.v1.Ratings
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class RatingsController : ControllerBase
    {
        private readonly IRepository<Rating, Guid> _repository;

        public RatingsController(IRepository<Rating, Guid> repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync(CancellationToken cancellationToken)
        {
            var rating = Rating.Create(
                score: new Score(5),
                description: "This is a rating description",
                reading: new Reading(
                    initiatedAt: DateTime.UtcNow.AddDays(-5),
                    finishedAt: DateTime.UtcNow
                ),
                userId: Guid.NewGuid(),
                bookId: Guid.NewGuid()
            );

            await _repository.AddAsync(rating, cancellationToken);

            return Created($"{rating.Id.Value}", rating);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken
        )
        {
            var rating = await _repository.GetByIdAsync(
                RatingId.Create(id),
                cancellationToken
            );

            return rating is not null ?
                Ok(rating) :
                NotFound();
        }
    }
}