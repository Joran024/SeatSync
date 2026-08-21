using Microsoft.OpenApi.Models;
namespace SeatSync.Api.Extensions;
public static class SwaggerExtensions
{
    public static IServiceCollection AddSeatSyncSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer(); services.AddSwaggerGen(o =>
        {
            o.SwaggerDoc("v1", new OpenApiInfo { Title = "SeatSync API", Version = "v1", Description = "Concurrency-safe reservation backend." });
            o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { Name = "Authorization", Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT", In = ParameterLocation.Header });
            o.AddSecurityRequirement(new OpenApiSecurityRequirement { [new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }] = Array.Empty<string>() });
        }); return services;
    }
}
