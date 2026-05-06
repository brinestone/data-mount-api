using System.Globalization;
using System.Text.Json;
using Asp.Versioning;
using DataMount.Api.AutoMapper;
using DataMount.Api.Options;
using DataMount.Api.Resources;
using DataMount.App.AutoMapper;
using DataMount.App.Extensions;
using DataMount.Infra.Contexts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Constants = DataMount.Api.Constants;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
var config = builder.Configuration;

builder.Services.Configure<ApiVersioningOptions>(config.GetSection("Versioning"));
builder.Services.AddApiVersioning(options =>
    {
        var opts = config.GetRequiredSection("Versioning").Get<VersioningOptions>()!;
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(opts.Major, opts.Minor);
        options.ReportApiVersions = true;
        options.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new HeaderApiVersionReader("x-api-version"),
            new QueryStringApiVersionReader("api-version")
        );
    })
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });
builder.Services.AddControllersWithViews()
    .AddDataAnnotationsLocalization()
    .AddJsonOptions(options =>
    {
        options.AllowInputFormatterExceptionMessages = true;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    })
    .AddViewLocalization();
builder.Services.Configure<ForwardedHeadersOptions>(options => { options.ForwardedHeaders = ForwardedHeaders.All; });
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitAllowedOrigins", policy =>
    {
        var origins = config.GetSection("AllowedOrigins").Get<string[]>();
        policy.WithOrigins(origins ?? [])
            .AllowAnyHeader()
            .AllowCredentials()
            .AllowAnyMethod();
    });
});
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new List<CultureInfo>
    {
        new("en"),
        new("fr"),
        new("en-US"),
        new("fr-FR"),
        new("fr-CM")
    };

    options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpLogging();
builder.Services.AddLogging();
builder.Services.AddDbContext<AuthContext<Guid>>(options =>
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
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(options =>
{
    options.Cookie.Name = Constants.AuthCookieName;
    options.SlidingExpiration = true;
    options.LoginPath = $"{Constants.ApiBasePath}/auth/sign-in";
    options.LogoutPath = $"{Constants.ApiBasePath}/api/auth/logout";
    options.Cookie.Path = Constants.ApiBasePath;
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    if (config["ASPNETCORE_ENVIRONMENT"] != "Development")
    {
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    }

    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
});
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.HttpOnly = true;
    if (config["ASPNETCORE_ENVIRONMENT"] != "Development")
    {
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    }

    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.Path = Constants.ApiBasePath;
    options.HeaderName = Constants.AntiForgeryHeaderName;
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
    app.MapGet("/api/health", () => new { ok = true })
        .WithTags("Health")
        .WithName("checkHealth")
        .WithSummary("Check health")
        .WithDescription("Health checking endpoint");
}
else
{
    app.MapGet("/api/health", () => new { ok = true });
}

var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);
app.UseForwardedHeaders(); // Move to the very top to fix IPs/Protocols early
app.UseCors("PermitAllowedOrigins");
app.UseRouting();

app.UseAuthentication(); // Identify the user
app.UseAuthorization(); // Check permissions
app.UseAntiforgery(); // Validate CSRF based on identified user

app.MapControllers();


app.Run();