using Microsoft.AspNetCore.Mvc;
using SeatSync.Application.Abstractions;
using SeatSync.Application.Contracts.Auth;
namespace SeatSync.Api.Controllers;
[ApiController, Route("api/auth")]
public sealed class AuthController(IAuthService auth) : ControllerBase
{
    [HttpPost("register")] public Task<AuthResponse> Register(RegisterRequest request, CancellationToken ct) => auth.RegisterAsync(request, ct);
    [HttpPost("login")] public Task<AuthResponse> Login(LoginRequest request, CancellationToken ct) => auth.LoginAsync(request, ct);
}
