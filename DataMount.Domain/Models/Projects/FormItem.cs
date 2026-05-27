namespace DataMount.Domain.Models.Projects;

public abstract class FormItem<TKey> : BaseEntity<TKey> where TKey : struct, IEquatable<TKey>
{
    public required string? Path { get; set; }
    public TKey FormId { get; set; }
    public virtual required Form<TKey> Form { get; set; }
}