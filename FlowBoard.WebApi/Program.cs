using FlowBoard.Application.Abstractions;
using FlowBoard.Connection.Persistence;
using FlowBoard.Persistence.Configurations;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection(DatabaseOptions.SectionName));

builder.Services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();

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

