using DataMount.Api.Filters;

namespace DataMount.Api.Extensions;

public static class ServiceExtensions
{
    public static void AddFilters(this IServiceCollection collection)
    {
        collection.AddScoped<AppExceptionFilter>();
    }
}