using System.ComponentModel.DataAnnotations;

namespace DataMount.Api.Payloads;

public class UserDto<TKey> : BaseEntityDto<TKey>
{
    public string? FirstName { get; set; }
    public string? Photo { get; set; }
    public bool IsBanned { get; set; }
    public bool IsOnboarded { get; set; }
    public string? BanReason { get; set; }
    public IList<ContactDto<TKey>> Contacts { get; set; } = [];
    public string? LastName { get; set; }
    public string FullName => string.Join(' ', FirstName ?? string.Empty, LastName ?? string.Empty).Trim();
}