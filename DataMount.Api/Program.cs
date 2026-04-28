using System.Text.Json;
using DataMount.App.AutoMapper;
using DataMount.App.Extensions;
using DataMount.Infra.Contexts;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
var config = builder.Configuration;


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
builder.Services.AddCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpLogging();
builder.Services.AddLogging();
builder.Services.AddDbContext<IdentityContext<Guid>>(options =>
{
    options.UseNpgsql(config.GetConnectionString("DefaultConnection"));
    options.UseSnakeCaseNamingConvention();
});
builder.Services.AddAutoMapper(c =>
{
    c.AddProfile<AutoMapperProfile<Guid>>();
    c.LicenseKey = config["Licenses:AutoMapper"];
});
builder.Services.AddApplicationServices<Guid>();

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

app.UseCors();
app.UseHttpLogging();
app.MapControllers();

app.Run();