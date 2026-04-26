namespace DataMount.Domain.Models;

public abstract class BaseEntity<TKey> where TKey : struct, IEquatable<TKey>
{
    public TKey? Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
