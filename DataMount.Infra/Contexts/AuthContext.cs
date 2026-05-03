using DataMount.Domain.Models;
using DataMount.Domain.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataMount.Infra.Contexts;

public class AuthContext<TKey>(DbContextOptions<AuthContext<TKey>> options)
    : DbContext(options) where TKey : struct, IEquatable<TKey>
{
    public virtual DbSet<User<TKey>> Users { get; set; }
    public virtual DbSet<Contact<TKey>> Contacts { get; set; }
    public virtual DbSet<CredentialAccount<TKey>> CredentialAccounts { get; set; }
    public virtual DbSet<LoginAttempt<TKey>> LoginAttempts { get; set; }
    public virtual DbSet<Session<TKey>> Sessions { get; set; }

    private static void ConfigureBase<T>(EntityTypeBuilder<T> builder) where T : BaseEntity<TKey>
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasDefaultValueSql("gen_random_uuid()").ValueGeneratedOnAdd();
        builder.Property(i => i.CreatedAt).HasDefaultValueSql("now()").ValueGeneratedOnAdd();
        builder.Property(i => i.UpdatedAt).HasDefaultValueSql("now()").ValueGeneratedOnAddOrUpdate();
    }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        mb.Entity<User<TKey>>(b =>
        {
            b.ToTable("users");
            ConfigureBase(b);
            b.Property(u => u.FirstName);
            b.Property(u => u.LastName).IsRequired();
            b.Ignore(u => u.IsOnboarded);
            b.HasMany(u => u.Contacts)
                .WithOne(c => c.Owner)
                .HasForeignKey(c => c.OwnerId)
                .IsRequired(false);
            b.HasMany(u => u.Accounts)
                .WithOne(a => a.Owner)
                .HasForeignKey(a => a.OwnerId)
                .IsRequired(false);
        });

        mb.Entity<Account<TKey>>(b =>
        {
            ConfigureBase(b);
            b.UseTphMappingStrategy();
            b.ToTable("accounts");
            b.HasDiscriminator<string>("account_type")
                .HasValue<OAuthAccount<TKey>>("oauth")
                .HasValue<CredentialAccount<TKey>>("credential");
            b.Property(a => a.IdentifierContactId).IsRequired();
            b.Ignore(a => a.IsBlocked);
            b.HasOne(a => a.Owner)
                .WithMany(u => u.Accounts)
                .HasForeignKey(a => a.OwnerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            b.HasAlternateKey(a => new { a.IdentifierContactId, a.OwnerId });
            b.HasOne(a => a.IdentifierContact)
                .WithMany()
                .HasForeignKey(a => a.IdentifierContactId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        mb.Entity<CredentialAccount<TKey>>(b =>
        {
            // b.ToTable("credential_accounts");
            b.Property(c => c.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);
        });
        mb.Entity<OAuthAccount<TKey>>(b =>
        {
            // b.ToTable("oauth_accounts");
            b.Property(o => o.AccessToken).IsRequired();
            b.Property(o => o.Provider).IsRequired();
        });
        mb.Entity<Contact<TKey>>(b =>
        {
            ConfigureBase(b);
            b.Ignore(c => c.Verified);
            b.ToTable("contacts");
            b.Property(c => c.Type).IsRequired();
            b.HasAlternateKey(c => new { c.Type, c.Value });
            b.Property(c => c.Value).IsRequired();
            b.HasOne(c => c.Owner)
                .WithMany(u => u.Contacts)
                .HasForeignKey(c => c.OwnerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });
        mb.Entity<LoginAttempt<TKey>>(b =>
        {
            b.Ignore(l => l.IsSuccess);
            b.ToTable("login_attempts");
            ConfigureBase(b);
            b.HasOne(l => l.Account)
                .WithMany()
                .HasForeignKey(l => l.AccountId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);
        });
        mb.Entity<Session<TKey>>(b =>
        {
            b.ToTable("sessions");
            ConfigureBase(b);
            b.Property(s => s.Ip).IsRequired(false);
            b.HasOne(s => s.Attempt)
                .WithOne()
                .HasForeignKey<Session<TKey>>(s => s.AttemptId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(s => s.Account)
                .WithMany()
                .HasForeignKey(s => s.AccountId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}