using BdmiAPI.Application.Genres;
using Microsoft.AspNetCore.Mvc;

namespace BdmiAPI.Api;

[ApiController]
[Route("api/genres")]
[Produces("application/json")]
public sealed class GenresController : ControllerBase
{
    private readonly IGenreService _svc;
    public GenresController(IGenreService svc) => _svc = svc;

    /// <summary>List genres (optional text filter)</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GenreListItemDto>), 200)]
    public async Task<IActionResult> List([FromQuery] string? q, CancellationToken ct)
        => Ok(await _svc.ListAsync(q, ct));

    /// <summary>Get one genre by id</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GenreDetailsDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
        => (await _svc.GetAsync(id, ct)) is { } dto ? Ok(dto) : NotFound();

    /// <summary>Create a new genre</summary>
    [HttpPost]
    [ProducesResponseType(typeof(GenreDetailsDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Create([FromBody] CreateGenreDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return ValidationProblem();
        try
        {
            var created = await _svc.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
        catch (GenreConflictException ex)
        {
            return Conflict(new { error = ex.Message }); // 409
        }
    }

    /// <summary>Update a genre</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateGenreDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return ValidationProblem();
        try
        {
            var ok = await _svc.UpdateAsync(id, dto, ct);
            return ok ? NoContent() : NotFound();
        }
        catch (GenreConflictException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    /// <summary>Delete a genre</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
        => (await _svc.DeleteAsync(id, ct)) ? NoContent() : NotFound();
}
