using System.Text.Json;
using DataMount.Domain.Models;
using DataMount.Domain.Models.Identity;
using DataMount.Domain.Models.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataMount.Infra.Contexts;

public class AppDbContext<TKey>(DbContextOptions<AppDbContext<TKey>> options)
    : DbContext(options) where TKey : struct, IEquatable<TKey>
{
    public virtual DbSet<User<TKey>> Users { get; set; }
    public virtual DbSet<Contact<TKey>> Contacts { get; set; }
    public virtual DbSet<CredentialAccount<TKey>> CredentialAccounts { get; set; }
    public virtual DbSet<LoginAttempt<TKey>> LoginAttempts { get; set; }
    public virtual DbSet<Session<TKey>> Sessions { get; set; }
    public virtual DbSet<Project<TKey>> Projects { get; set; }
    public virtual DbSet<Form<TKey>> Forms { get; set; }
    public virtual DbSet<Organization<TKey>> Organizations { get; set; }
    public virtual DbSet<OrganizationMembership<TKey>> OrganizationMemberships { get; set; }

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

        mb.Entity<TextQuestion<TKey>>(b =>
        {
            b.Property(c => c.Config)
                .HasConversion(
                    v => JsonSerializer.Serialize(v),
                    v => JsonSerializer.Deserialize<TextQuestionConfig<TKey>>(v)!
                )
                .HasColumnType("jsonb");
        });

        mb.Entity<FormItem<TKey>>(b =>
        {
            ConfigureBase(b);
            b.ToTable("form_items");
            b.Property(f => f.Path).IsRequired();
            b.HasIndex(f => new { f.FormId, f.Path }).IsUnique();
            b.HasOne(f => f.Form)
                .WithMany(f => f.FormItems)
                .HasForeignKey(f => f.FormId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            b.HasDiscriminator<string>("item_type")
                .HasValue<TextQuestion<TKey>>("question");
        });

        mb.Entity<Form<TKey>>(b =>
        {
            ConfigureBase(b);
            b.ToTable("forms");
            b.Property(f => f.Title).IsRequired();

            b.HasOne(f => f.CreatedBy)
                .WithMany()
                .HasForeignKey(f => f.CreatedById);
            b.HasOne(f => f.Organization)
                .WithMany()
                .HasForeignKey(f => f.OrganizationId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(f => f.Project)
                .WithMany(p => p.Forms)
                .HasForeignKey(f => f.ProjectId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            b.HasMany(f => f.FormItems)
                .WithOne(f => f.Form)
                .HasForeignKey(f => f.FormId)
                .IsRequired(false);
        });

        mb.Entity<Project<TKey>>(b =>
        {
            ConfigureBase(b);
            b.ToTable("projects");
            b.Property(p => p.Name).IsRequired();
            b.HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.SetNull);
            b.HasOne(p => p.Organization)
                .WithMany(o => o.Projects)
                .IsRequired()
                .HasForeignKey(p => p.OrganizationId);
            b.HasMany(p => p.Forms)
                .WithOne(f => f.Project)
                .HasForeignKey(f => f.ProjectId)
                .IsRequired(false);
        });

        #region Identity Models

        mb.Entity<OrganizationMembership<TKey>>(b =>
        {
            b.ToTable("org_memberships");
            ConfigureBase(b);
            b.Property(o => o.PermissionString).IsRequired();
            b.HasOne(o => o.User)
                .WithMany()
                .IsRequired()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(o => o.Organization)
                .WithMany(o => o.Memberships)
                .HasForeignKey(o => o.OrganizationId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(o => new { o.UserId, o.OrganizationId }).IsUnique();
        });
        mb.Entity<Organization<TKey>>(b =>
        {
            b.ToTable("organizations");
            ConfigureBase(b);
            b.Property(o => o.Name).IsRequired();
            b.HasMany(o => o.Memberships)
                .WithOne(m => m.Organization)
                .HasForeignKey(m => m.OrganizationId)
                .IsRequired(false);
        });
        mb.Entity<User<TKey>>(b =>
        {
            b.ToTable("users");
            ConfigureBase(b);
            b.Property(u => u.FirstName);
            b.Ignore(u => u.IsBanned);
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

        #endregion
    }
}