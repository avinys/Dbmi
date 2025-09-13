using BdmiAPI.DTOs;

namespace BdmiAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<IReadOnlyList<UserListItemDto>> ListAsync(string? q, CancellationToken ct = default);
        Task<UserDetailsDto?> GetAsync(int id, CancellationToken ct = default);
        Task<UserDetailsDto> CreateAsync(CreateUserDto dto, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, UpdateUserDto dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> AnonymizeAndDeleteAsync(int id, CancellationToken ct = default);
    }

    public sealed class UserConflictException : Exception
    {
        public UserConflictException(string message) : base(message) { }
    }

    public sealed class UserForbiddenOperationException : Exception
    {
        public UserForbiddenOperationException(string message) : base(message) { }
    }
}
