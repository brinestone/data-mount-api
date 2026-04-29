using System.Text.Json;
using DataMount.Api.AutoMapper;
using DataMount.App.AutoMapper;
using DataMount.App.Extensions;
using DataMount.Infra.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Constants = DataMount.Api.Constants;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
var config = builder.Configuration;


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
builder.Services.Configure<ForwardedHeadersOptions>(options => { options.ForwardedHeaders = ForwardedHeaders.All; });
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
    c.AddProfile<AppMapperProfile<Guid>>();
    c.AddProfile<PayloadMapperProfile<Guid>>();
    c.LicenseKey = config["Licenses:AutoMapper"];
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddCookie(options =>
{
    options.Cookie.Name = Constants.AuthCookieName;
    options.SlidingExpiration = true;
    options.LoginPath = "/api/identity/sign-in";
    options.LogoutPath = "/api/identity/logout";
    options.Cookie.Path = "/api";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
});
builder.Services.AddAuthorization();
builder.Services.AddApplicationServices<Guid>();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes.Add("CookieAuth", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Cookie,
            Name = Constants.AuthCookieName, // Ensure this matches your actual cookie name
            Description = "Log in via the /api/identity/login endpoint to set the cookie."
        });
        return Task.CompletedTask;
    });
});
var app = builder.Build();

app.UseHttpLogging();
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
        options.WithDotNetFlag();
        options.AddPreferredSecuritySchemes(Constants.AuthCookieName);
    });
}

app.UseCors();
app.UseForwardedHeaders();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();