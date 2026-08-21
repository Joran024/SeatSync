using SeatSync.Application.Contracts.Auth;
namespace SeatSync.Application.Abstractions;
public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct);
}
