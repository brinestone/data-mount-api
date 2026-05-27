namespace DataMount.Domain.Models.Identity;

public class OrganizationMembership<TKey> : BaseEntity<TKey> where TKey : struct, IEquatable<TKey>
{
    public required string? PermissionString { get; set; }

    public required TKey? UserId { get; set; }
    public required TKey? OrganizationId { get; set; }

    public required Organization<TKey> Organization { get; set; }
    public virtual User<TKey>? User { get; set; }
}