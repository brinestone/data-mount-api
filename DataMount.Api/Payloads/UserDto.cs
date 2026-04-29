using System.ComponentModel.DataAnnotations;

namespace DataMount.Api.Payloads;

public class UserDto<TKey> : BaseEntityDto<TKey>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    [Required]
    public string FullName => string.Join(' ', FirstName ?? string.Empty, LastName ?? string.Empty).Trim();
}