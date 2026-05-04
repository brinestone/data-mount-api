using AutoMapper;
using DataMount.App.Inputs;
using DataMount.App.Services.Contracts;
using DataMount.Domain.Exceptions;
using DataMount.Domain.Models.Identity;
using DataMount.Infra.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataMount.App.Services.Implementations;

public class AuthService<TKey>(
    ILogger<AuthService<TKey>> logger,
    AuthContext<TKey> context,
    IPasswordEncoder passwordEncoder,
    IMapper mapper) : IAuthService<TKey>
    where TKey : struct, IEquatable<TKey>
{
    public async Task<Session<TKey>> CreateUserCredentialSessionAsync(CreateCredentialSessionInput input,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("creating session with credential identifier: {id}", input.Identifier);

        var attempt = new LoginAttempt<TKey>
        {
            Ip = input.Ip,
            UserAgent = input.UserAgent,
        };
        await context.LoginAttempts.AddAsync(attempt, cancellationToken);

        logger.LogDebug("finding credential account: {identifier}", input.Identifier);

        var result = await context.CredentialAccounts
            .Join(context.Contacts, account => account.IdentifierContactId, contact => contact.Id,
                (account, contact) => new { contact, account })
            .Where(pair => pair.contact.Value == input.Identifier && pair.contact.Type == input.ContactType)
            .Select(pair => pair.account)
            .Include(account => account.Owner)
            .FirstOrDefaultAsync(cancellationToken);
        if (result is null)
        {
            var ex = new AccountNotFoundException();
            attempt.FailureReason = ex.Message;
            attempt.FailedAt = DateTime.Now;
            await context.SaveChangesAsync(cancellationToken);
            throw ex;
        }

        if (result.Owner?.IsBanned ?? false)
        {
            var ex = new ForbiddenException(result.Owner.BanReason);
            attempt.FailureReason = ex.Message;
            attempt.FailedAt = DateTime.Now;
            await context.SaveChangesAsync(cancellationToken);
            throw ex;
        }

        var isPasswordValid = passwordEncoder.Verify(input.Password, result.PasswordHash);
        if (!isPasswordValid)
        {
            var ex = new UnauthorizedException("Invalid identifier or password");
            attempt.FailureReason = ex.Message;
            attempt.FailedAt = DateTime.Now;
            await context.SaveChangesAsync(cancellationToken);
            throw ex;
        }

        var session = new Session<TKey>
        {
            UserAgent = input.UserAgent,
            Ip = input.Ip,
            Account = result,
            Attempt = attempt
        };
        attempt.Account = result;
        await context.Sessions.AddAsync(session, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return session;
    }

    public async Task<User<TKey>> CreateUserFromCredentialAsync(CreateUserWithCredentialInput input,
        CancellationToken token = default)
    {
        var accountExists = await context.Contacts
            .AnyAsync(contact => contact.Type == input.ContactType && contact.Value == input.Identifier, token);
        if (accountExists)
            throw new ConflictException($"{input.Identifier} is already in use");
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