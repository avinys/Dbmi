using BdmiAPI.DTOs;
using BdmiAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BdmiAPI.Api
{
    [ApiController]
    [Route("api/users")]
    [Produces("application/json")]
    public sealed class UsersController : ControllerBase
    {
        private readonly IUserService _svc;
        public UsersController(IUserService svc) => _svc = svc;

        /// <summary>List users (optional query q matches username or email)</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserListItemDto>), 200)]
        public async Task<IActionResult> List([FromQuery] string? q, CancellationToken ct)
            => Ok(await _svc.ListAsync(q, ct));

        /// <summary>Get a single user</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(UserDetailsDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id, CancellationToken ct)
            => (await _svc.GetAsync(id, ct)) is { } dto ? Ok(dto) : NotFound();

        /// <summary>Create a user</summary>
        [HttpPost]
        [ProducesResponseType(typeof(UserDetailsDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem();
            try
            {
                var created = await _svc.CreateAsync(dto, ct);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (UserConflictException ex) { return Conflict(new { error = ex.Message }); }
        }

        /// <summary>Update a user</summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem();
            try
            {
                var ok = await _svc.UpdateAsync(id, dto, ct);
                return ok ? NoContent() : NotFound();
            }
            catch (UserConflictException ex) { return Conflict(new { error = ex.Message }); }
        }

        /// <summary>Delete a user (anonymizes content first)</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            try
            {
                var ok = await _svc.DeleteAsync(id, ct);
                return ok ? NoContent() : NotFound();
            }
            catch (UserForbiddenOperationException ex) { return Conflict(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
        }
    }
}
