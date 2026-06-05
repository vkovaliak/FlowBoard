using FlowBoard.Database;
using FlowBoard.Application.Extensions;
using FlowBoard.Persistence.Extensions;
using FlowBoard.Persistence.Configurations;
using FlowBoard.Infrastructure.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FlowBoard.Infrastructure.Extensions;
using FlowBoard.WebApi.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DatabaseOptions>(
    builder.Configuration.GetSection(DatabaseOptions.SectionName));
builder.Services.AddSingleton<DatabaseInitializer>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddPersistence();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddHttpContextAccessor();

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.SectionName));

var jwtOptions = builder.Configuration
    .GetSection(JwtOptions.SectionName)
    .Get<JwtOptions>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions!.Issuer,
            ValidAudience = jwtOptions!.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions!.SecretKey))
        };
    });

var corsOptions = builder.Configuration
    .GetSection(CorsOptions.SectionName)
    .Get<CorsOptions>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsOptions!.PolicyName, policy =>
    {
        policy.WithOrigins(corsOptions.AllowedOrigin)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await initializer.InitializeAsync();
}

app.UseHttpsRedirection();

app.UseCors(corsOptions!.PolicyName);

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.Run();
