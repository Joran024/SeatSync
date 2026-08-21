namespace SeatSync.Infrastructure.Auth;
public sealed class JwtOptions { public const string SectionName = "Jwt"; public string Issuer { get; init; } = "SeatSync.Api"; public string Audience { get; init; } = "SeatSync.Client"; public string Key { get; init; } = string.Empty; public int ExpirationMinutes { get; init; } = 180; }
