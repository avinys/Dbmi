using BdmiAPI.DTOs;
using BdmiAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BdmiAPI.Api
{
    [ApiController]
    [Route("api/reviews")]
    [Produces("application/json")]
    public sealed class ReviewsController : ControllerBase
    {
        private readonly IReviewService _svc;
        public ReviewsController(IReviewService svc) => _svc = svc;

        /// <summary>List reviews (filter by movieId and/or userId)</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ReviewListItemDto>), 200)]
        public async Task<IActionResult> List([FromQuery] int? movieId, [FromQuery] int? userId, CancellationToken ct)
            => Ok(await _svc.ListAsync(movieId, userId, ct));

        /// <summary>Get a single review</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ReviewDetailsDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id, CancellationToken ct)
            => (await _svc.GetAsync(id, ct)) is { } dto ? Ok(dto) : NotFound();

        /// <summary>Create a review (one per user per movie)</summary>
        [HttpPost]
        [ProducesResponseType(typeof(ReviewDetailsDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> Create([FromBody] CreateReviewDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem();

            try
            {
                var created = await _svc.CreateAsync(dto, ct);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (ReviewValidationException ex) { return UnprocessableEntity(new { error = ex.Message }); }
            catch (ReviewConflictException ex) { return Conflict(new { error = ex.Message }); }
        }

        /// <summary>Update a review (idempotent)</summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateReviewDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem();
            var ok = await _svc.UpdateAsync(id, dto, ct);
            return ok ? NoContent() : NotFound();
        }

        /// <summary>Delete a review</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
            => (await _svc.DeleteAsync(id, ct)) ? NoContent() : NotFound();
    }
}
