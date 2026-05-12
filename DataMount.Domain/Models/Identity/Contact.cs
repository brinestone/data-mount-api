namespace DataMount.Domain.Models.Identity;

public enum ContactType
{
    Email,
    Phone
}

public class Contact<TKey> : BaseEntity<TKey> where TKey : struct, IEquatable<TKey>
{
    public bool Verified => VerifiedAt is not null;
    public DateTime? VerifiedAt { get; set; }
    public ContactType Type { get; set; }
    public string? Value { get; set; }
    public TKey? OwnerId { get; set; }

    public virtual User<TKey>? Owner { get; set; }
}