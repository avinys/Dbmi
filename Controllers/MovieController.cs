using BdmiAPI.DTOs;
using BdmiAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BdmiAPI.Controllers
{
    public class MovieController
    {
        [ApiController]
        [Route("api/movies")]
        [Produces("application/json")]
        public sealed class MoviesController : ControllerBase
        {
            private readonly IMovieService _svc;
            public MoviesController(IMovieService svc) => _svc = svc;

            /// <summary>List movies with optional filters: genreId, q; sort: title|rating|date</summary>
            [HttpGet]
            [ProducesResponseType(typeof(IEnumerable<MovieListItemDto>), 200)]
            public async Task<IActionResult> List([FromQuery] int? genreId, [FromQuery] string? q, [FromQuery] string? sort, CancellationToken ct)
                => Ok(await _svc.ListAsync(genreId, q, sort, ct));

            /// <summary>Get movie details</summary>
            [HttpGet("{id:int}")]
            [ProducesResponseType(typeof(MovieDetailsDto), 200)]
            [ProducesResponseType(404)]
            public async Task<IActionResult> Get(int id, CancellationToken ct)
                => (await _svc.GetAsync(id, ct)) is { } dto ? Ok(dto) : NotFound();

            /// <summary>Create new movie</summary>
            [HttpPost]
            [ProducesResponseType(typeof(MovieDetailsDto), 201)]
            [ProducesResponseType(400)]
            [ProducesResponseType(409)]
            [ProducesResponseType(422)]
            public async Task<IActionResult> Create([FromBody] CreateMovieDto dto, CancellationToken ct)
            {
                if (!ModelState.IsValid) return ValidationProblem();
                try
                {
                    var created = await _svc.CreateAsync(dto, ct);
                    return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
                }
                catch (MovieValidationException ex) { return UnprocessableEntity(new { error = ex.Message }); }
                catch (MovieConflictException ex) { return Conflict(new { error = ex.Message }); }
            }

            /// <summary>Update a movie (idempotent)</summary>
            [HttpPut("{id:int}")]
            [ProducesResponseType(204)]
            [ProducesResponseType(404)]
            [ProducesResponseType(409)]
            [ProducesResponseType(422)]
            public async Task<IActionResult> Update(int id, [FromBody] UpdateMovieDto dto, CancellationToken ct)
            {
                if (!ModelState.IsValid) return ValidationProblem();
                try
                {
                    var ok = await _svc.UpdateAsync(id, dto, ct);
                    return ok ? NoContent() : NotFound();
                }
                catch (MovieValidationException ex) { return UnprocessableEntity(new { error = ex.Message }); }
                catch (MovieConflictException ex) { return Conflict(new { error = ex.Message }); }
            }

            /// <summary>Delete a movie</summary>
            [HttpDelete("{id:int}")]
            [ProducesResponseType(204)]
            [ProducesResponseType(404)]
            public async Task<IActionResult> Delete(int id, CancellationToken ct)
                => (await _svc.DeleteAsync(id, ct)) ? NoContent() : NotFound();

            /// <summary>Hierarchical: all reviews for a movie (optionally include text)</summary>
            [HttpGet("{id:int}/reviews")]
            [ProducesResponseType(typeof(object), 200)]
            [ProducesResponseType(404)]
            public async Task<IActionResult> Reviews(int id, [FromQuery] bool includeText = false, CancellationToken ct = default)
            {
                var payload = await _svc.GetReviewsForMovieAsync(id, includeText, ct);
                return payload is null ? NotFound() : Ok(payload);
            }
        }
    }
}
