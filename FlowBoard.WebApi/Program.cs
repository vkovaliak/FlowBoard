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
using FlowBoard.WebApi.Hubs;
using FlowBoard.Domain.Constants;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DatabaseOptions>(
    builder.Configuration.GetSection(DatabaseOptions.SectionName));
builder.Services.AddSingleton<DatabaseInitializer>();

builder.Services.Configure<AzureBlobOptions>(
    builder.Configuration.GetSection(AzureBlobOptions.SectionName));

builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddPersistence();

builder.Services.AddSignalR();
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter()));;
        
builder.Services.AddOpenApi();

builder.Services.AddHttpContextAccessor();

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.SectionName));

var jwtOptions = builder.Configuration
    .GetSection(JwtOptions.SectionName)
    .Get<JwtOptions>();

builder.Services.Configure<EntraIdOptions>(
    builder.Configuration.GetSection(EntraIdOptions.SectionName));

var entraOptions = builder.Configuration
    .GetSection(EntraIdOptions.SectionName)
    .Get<EntraIdOptions>();

builder.Services.Configure<FrontendOptions>(
    builder.Configuration.GetSection(FrontendOptions.SectionName));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
})
.AddCookie(ExternalAuthConstants.ExternalCookieScheme)
.AddOpenIdConnect(ExternalAuthConstants.MicrosoftScheme, options =>
{
    options.Authority = entraOptions!.Authority;
    options.ClientId = entraOptions.ClientId;
    options.ClientSecret = entraOptions.ClientSecret;
    options.CallbackPath = entraOptions.CallbackPath;

    options.ResponseType = ExternalAuthConstants.ResponseTypeCode;
    options.SignInScheme = ExternalAuthConstants.ExternalCookieScheme;

    options.Scope.Clear();
    options.Scope.Add(ExternalAuthConstants.ScopeOpenId);
    options.Scope.Add(ExternalAuthConstants.ScopeProfile);
    options.Scope.Add(ExternalAuthConstants.ScopeEmail);

    options.SaveTokens = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ExternalAuthConstants.NameClaimType,
        ValidateIssuer = false
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

app.MapHub<BoardHub>(HubRoutes.Boards);
app.MapHub<CommentHub>(HubRoutes.Comments);
app.MapControllers();
app.Run();
