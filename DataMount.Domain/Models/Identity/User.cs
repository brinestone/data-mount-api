namespace DataMount.Domain.Models.Identity;

public class User<TKey> : BaseEntity<TKey> where TKey : struct, IEquatable<TKey>
{
    public string? FirstName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? Photo { get; set; }
    public DateTime? BannedAt { get; set; }
    public string? BanReason { get; set; }

    public virtual IList<Contact<TKey>> Contacts { get; set; } = [];
    public virtual IList<Account<TKey>> Accounts { get; set; } = [];
}
