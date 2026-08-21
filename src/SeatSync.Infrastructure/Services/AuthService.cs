using Microsoft.AspNetCore.Identity;
using SeatSync.Application.Abstractions;
using SeatSync.Application.Contracts.Auth;
using SeatSync.Application.Exceptions;
using SeatSync.Infrastructure.Auth;
namespace SeatSync.Infrastructure.Services;
public sealed class AuthService(UserManager<ApplicationUser> users, JwtTokenService tokens) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        var user = new ApplicationUser { UserName = request.Email.Trim().ToLowerInvariant(), Email = request.Email.Trim().ToLowerInvariant(), DisplayName = request.DisplayName.Trim() };
        var result = await users.CreateAsync(user, request.Password);
        if (!result.Succeeded) throw new ConflictException(string.Join(" ", result.Errors.Select(x => x.Description)));
        return ToResponse(user);
    }
    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var user = await users.FindByEmailAsync(request.Email.Trim());
        if (user is null || !await users.CheckPasswordAsync(user, request.Password)) throw new UnauthorizedException("Invalid email or password.");
        return ToResponse(user);
    }
    private AuthResponse ToResponse(ApplicationUser user) => new(user.Id, user.Email!, user.DisplayName, tokens.Create(user));
}
