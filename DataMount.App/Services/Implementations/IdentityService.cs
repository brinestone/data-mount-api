using AutoMapper;
using DataMount.App.Inputs;
using DataMount.App.Services.Contracts;
using DataMount.Domain.Models.Identity;
using DataMount.Infra.Contexts;
using Microsoft.Extensions.Logging;

namespace DataMount.App.Services.Implementations;

public class IdentityService<TKey>(
    ILogger<IdentityService<TKey>> logger,
    IdentityContext<TKey> context,
    IPasswordEncoder passwordEncoder,
    IMapper mapper) : IIdentityService<TKey>
    where TKey : struct, IEquatable<TKey>
{
    public async Task<User<TKey>> CreateUserFromCredentialAsync(CreateUserWithCredentialInput input,
        CancellationToken token = default)
    {
        logger.LogInformation("creating new user using credentials. type: {0}, identifier: {1}", input.ContactType,
            input.Identifier);

        var user = mapper.Map<User<TKey>>(input);
        var contact = new Contact<TKey>
        {
            Owner = user,
            Type = input.ContactType,
            Value = input.Identifier
        };

        var ph = passwordEncoder.Hash(input.Password);
        var account = new CredentialAccount<TKey>
        {
            PasswordHash = ph,
            Owner = user,
            IdentifierContact = contact
        };

        await context.Users.AddAsync(user, token);
        await context.Contacts.AddAsync(contact, token);
        await context.CredentialAccounts.AddAsync(account, token);
        await context.SaveChangesAsync(token);

        logger.LogInformation("User created successfully.");
        return user;
    }
}