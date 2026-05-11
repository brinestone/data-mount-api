using DataMount.Domain.Models.Identity;

namespace DataMount.App.Services.Contracts;

public interface IUserService<TKey> where TKey : struct, IEquatable<TKey>
{
    Task<User<TKey>?> FindUserByIdAsync(TKey id, CancellationToken token = default);
}