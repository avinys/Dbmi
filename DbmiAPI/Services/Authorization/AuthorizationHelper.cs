using BdmiAPI.Models;
using System.Security.Claims;

namespace BdmiAPI.Services.Authorization
{
    public static class AuthorizationHelper
    {
        public static bool IsAdmin(ClaimsPrincipal user)
            => user.IsInRole(UserRole.Admin.ToString());

        public static bool IsAuthenticated(ClaimsPrincipal user)
            => user.Identity?.IsAuthenticated ?? false;

        public static int? GetUserId(ClaimsPrincipal user)
            => int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

        public static bool CanModifyResource(ClaimsPrincipal user, int resourceOwnerId)
            => IsAdmin(user) || GetUserId(user) == resourceOwnerId;
    }
}