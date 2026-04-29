using DataMount.App.Inputs;
using DataMount.Domain.Models.Identity;

namespace DataMount.App.Services.Contracts;

public interface IIdentityService<TKey> where TKey : struct, IEquatable<TKey>
{
    Task<Session<TKey>> CreateUserCredentialSessionAsync(CreateCredentialSessionInput input,
        CancellationToken cancellationToken = default);

    Task<User<TKey>> CreateUserFromCredentialAsync(CreateUserWithCredentialInput input,
        CancellationToken cancellationToken = default);
}