using DataMount.Infra.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("Error: Connection string not found.");
    return 1;
}

var services = new ServiceCollection();
services.AddDbContext<IdentityContext<Guid>>(options => options.UseNpgsql(connectionString));

var sp = services.BuildServiceProvider();

using var scope = sp.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<IdentityContext<Guid>>();

Console.WriteLine("Applying migrations...");

try
{
    await db.Database.MigrateAsync();
    Console.WriteLine("Migrations applied successfully.");
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine($"Migration failed: {ex.Message}");
    return 1;
}