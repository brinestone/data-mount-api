using DataMount.App.Services.Contracts;
using DataMount.Domain.Models.Identity;
using DataMount.Infra.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataMount.App.Services.Implementations;

public class UserServiceV1<TKey>(IdentityContext<TKey> context)
    : IUserService<TKey> where TKey : struct, IEquatable<TKey>
{
    public async Task<User<TKey>?> FindUserByIdAsync(TKey id, CancellationToken token = default)
    {
        return await context.Users
            .Include(u => u.Contacts)
            .FirstOrDefaultAsync(u => id.Equals(u.Id), token);
    }
}