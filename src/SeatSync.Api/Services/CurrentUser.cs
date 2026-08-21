using System.Security.Claims;
using SeatSync.Application.Abstractions;
namespace SeatSync.Api.Services;
public sealed class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public Guid UserId => Guid.TryParse(accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : throw new UnauthorizedAccessException("Authenticated user id is missing.");
}
