using DataMount.Domain.Models.Identity;

namespace DataMount.Domain.Models.Projects;

public class Project<TKey> : BaseEntity<TKey> where TKey : struct, IEquatable<TKey>
{
    public required string? Name { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public string? Description { get; set; }

    public TKey? CreatedById { get; set; }
    public OrganizationMembership<TKey>? CreatedBy { get; set; }

    public required TKey? OrganizationId { get; set; }
    public virtual Organization<TKey>? Organization { get; set; }

    public virtual ISet<Form<TKey>> Forms { get; set; } = new HashSet<Form<TKey>>();
}