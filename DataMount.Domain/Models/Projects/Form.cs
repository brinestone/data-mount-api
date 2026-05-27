using DataMount.Domain.Models.Identity;

namespace DataMount.Domain.Models.Projects;

public class Form<TKey> : BaseEntity<TKey> where TKey : struct, IEquatable<TKey>
{
    public required string? Title { get; set; }
    public TKey? CreatedById { get; set; }
    public required TKey? ProjectId { get; set; }
    public required TKey? OrganizationId { get; set; }
    public DateTime? ArchivedAt { get; set; }

    public virtual Organization<TKey>? Organization { get; set; }
    public virtual OrganizationMembership<TKey>? CreatedBy { get; set; }
    public virtual Project<TKey>? Project { get; set; }
    public virtual ISet<FormItem<TKey>> FormItems { get; set; } = new HashSet<FormItem<TKey>>();
}