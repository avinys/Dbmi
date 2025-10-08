using BdmiAPI.DTOs;
using BdmiAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<ReviewListItemDto>), 200)]
        public async Task<IActionResult> List([FromQuery] int? movieId, [FromQuery] int? userId, CancellationToken ct)
        {
            try 
            {
                return Ok(await _svc.ListAsync(movieId, userId, User, ct));
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        /// <summary>Get a single review</summary>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ReviewDetailsDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id, CancellationToken ct)
        {
            try 
            {
                var dto = await _svc.GetAsync(id, User, ct);
                return dto is not null ? Ok(dto) : NotFound();
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        /// <summary>Create a review (one per user per movie)</summary>
        [Authorize]
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
                var created = await _svc.CreateAsync(dto, User, ct);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (ReviewValidationException ex) { return UnprocessableEntity(new { error = ex.Message }); }
            catch (ReviewConflictException ex) { return Conflict(new { error = ex.Message }); }
        }

        /// <summary>Update a review (idempotent)</summary>
        [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateReviewDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem();
            try
            {
                var ok = await _svc.UpdateAsync(id, dto, User, ct);
                return ok ? NoContent() : NotFound();
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        /// <summary>Delete a review</summary>
        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            try
            {
                return await _svc.DeleteAsync(id, User, ct) ? NoContent() : NotFound();
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }
    }
}
