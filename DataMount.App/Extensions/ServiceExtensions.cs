using DataMount.App.Services;
using DataMount.App.Services.Contracts;
using DataMount.App.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace DataMount.App.Extensions;

public static class ServiceExtensions
{
    public static void AddApplicationServices<TKey>(this IServiceCollection collection)
        where TKey : struct, IEquatable<TKey>
    {
        collection.AddSingleton<IPasswordEncoder, Argon2PasswordEncoder>();
        collection.AddScoped<IAuthService<TKey>, AuthService<TKey>>();
        collection.AddScoped<IUserService<TKey>, UserServiceV1<TKey>>();
    }
}