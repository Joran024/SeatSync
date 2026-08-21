using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SeatSync.Application.Abstractions;
using SeatSync.Infrastructure.Auth;
using SeatSync.Infrastructure.Background;
using SeatSync.Infrastructure.Persistence;
using SeatSync.Infrastructure.Services;
using StackExchange.Redis;
namespace SeatSync.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(o => o.UseSqlServer(config.GetConnectionString("Database")));
        services.AddIdentityCore<ApplicationUser>(o => { o.Password.RequiredLength = 10; o.Password.RequireDigit = true; o.Password.RequireUppercase = false; o.Password.RequireNonAlphanumeric = false; }).AddEntityFrameworkStores<AppDbContext>();
        services.Configure<JwtOptions>(config.GetSection(JwtOptions.SectionName)); var jwt = config.GetSection(JwtOptions.SectionName).Get<JwtOptions>()!;
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters { ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true, ValidateIssuerSigningKey = true, ValidIssuer = jwt.Issuer, ValidAudience = jwt.Audience, IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)) });
        services.AddAuthorization(); services.AddSingleton<JwtTokenService>();
        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(config.GetConnectionString("Redis")!));
        services.AddScoped<IAuthService, AuthService>(); services.AddScoped<IEventService, EventService>(); services.AddScoped<IReservationService, ReservationService>(); services.AddScoped<ISeatAvailabilityCache, SeatAvailabilityCache>();
        services.AddHostedService<HoldExpiryService>(); return services;
    }
}
