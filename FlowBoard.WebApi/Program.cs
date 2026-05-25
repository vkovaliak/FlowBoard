using FlowBoard.Application.Abstractions;
using FlowBoard.Connection.Persistence;
using FlowBoard.WebApi.Settings;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("ConnectionStrings"));

builder.Services.AddSingleton<ISqlConnectionFactory>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    
    if (string.IsNullOrEmpty(settings.DefaultConnection))
    {
        throw new InvalidOperationException("Connection string is empty!");
    }
    
    return new SqlConnectionFactory(settings.DefaultConnection);
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

