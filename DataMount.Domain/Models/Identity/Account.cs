namespace DataMount.Domain.Models.Identity;

public abstract class Account<TKey> : BaseEntity<TKey> where TKey : struct, IEquatable<TKey>
{
    public TKey? OwnerId { get; set; }
    public DateTime? BlockedAt { get; set; }
    public string? BlockReason { get; set; }
    public bool IsBlocked => BlockedAt.HasValue;
    public TKey? IdentifierContactId { get; set; }
    public Contact<TKey>? IdentifierContact { get; set; }

    public virtual User<TKey>? Owner { get; set; }
}