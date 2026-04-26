using DataMount.Infra.Contexts;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateSlimBuilder(args);
var config = builder.Configuration;

builder.Services.AddDbContext<IdentityContext<Guid>>(options =>
{
    options.UseNpgsql(config.GetConnectionString("DefaultConnection"));
    options.UseSnakeCaseNamingConvention();
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("DataMount API Reference")
            .DisableMcp()
            .DisableAgent()
            .WithTheme(ScalarTheme.BluePlanet)
            .ShowOperationId()
            .SortTagsAlphabetically();
    });
}

app.Run();