using DataMount.Domain.Models.Projects;

namespace DataMount.Domain.Models.Identity;

public class Organization<TKey> : BaseEntity<TKey> where TKey : struct, IEquatable<TKey>
{
    public required string? Name { get; set; }

    public List<OrganizationMembership<TKey>> Memberships { get; set; } = [];
    public List<Project<TKey>> Projects { get; set; } = [];
}