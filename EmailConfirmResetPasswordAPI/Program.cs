using EmailConfirmResetPasswordAPI.Helper;
using EmailConfirmResetPasswordAPI.Interfaces;
using EmailConfirmResetPasswordAPI.Models;
using EmailConfirmResetPasswordAPI.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.Configure<AppsettingEmail>(configuration.GetSection(nameof(AppsettingEmail)));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IHelper, Helper>();
builder.Services.AddScoped<IUserRegistrationRepository, UserRegistrationRepository>();
builder.Services.AddScoped<AppsettingEmail>();

builder.Services.AddScoped(v => v.GetRequiredService<IOptions<AppsettingEmail>>().Value);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
