using BdmiAPI.DTOs;
using BdmiAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<UserListItemDto>), 200)]
        public async Task<IActionResult> List([FromQuery] string? q, CancellationToken ct)
        {
            try
            {
                return Ok(await _svc.ListAsync(q, User, ct));
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        /// <summary>Get a single user</summary>
        [HttpGet("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(UserDetailsDto), 200)]
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

        /// <summary>Update a user</summary>
        [HttpPut("{id:int}")]
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem();
            try
            {
                var ok = await _svc.UpdateAsync(id, dto, User, ct);
                return ok ? NoContent() : NotFound();
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (UserConflictException ex) { return Conflict(new { error = ex.Message }); }
        }

        /// <summary>Delete a user (anonymizes content first)</summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            try
            {
                var ok = await _svc.DeleteAsync(id, User, ct);
                return ok ? NoContent() : NotFound();
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (UserForbiddenOperationException ex) { return Conflict(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
        }
    }
}
