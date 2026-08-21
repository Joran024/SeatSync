using FluentValidation.AspNetCore;
using SeatSync.Api.Extensions;
using SeatSync.Api.Middleware;
using SeatSync.Api.Services;
using SeatSync.Application;
using SeatSync.Application.Abstractions;
using SeatSync.Infrastructure;
using SeatSync.Infrastructure.Persistence;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddFluentValidation(o => o.RegisterValidatorsFromAssemblyContaining<SeatSync.Application.Validation.RegisterRequestValidator>());
builder.Services.AddHttpContextAccessor(); builder.Services.AddScoped<ICurrentUser, CurrentUser>(); builder.Services.AddApplication(); builder.Services.AddInfrastructure(builder.Configuration); builder.Services.AddSeatSyncSwagger();
var app = builder.Build(); app.UseMiddleware<ExceptionHandlingMiddleware>();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseHttpsRedirection(); app.UseAuthentication(); app.UseAuthorization(); app.MapControllers(); await app.Services.SeedSeatSyncAsync(); await app.RunAsync();
