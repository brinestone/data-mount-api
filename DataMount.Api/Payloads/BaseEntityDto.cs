using System.ComponentModel.DataAnnotations;

namespace DataMount.Api.Payloads;

public abstract class BaseEntityDto<TKey>
{
    [Required]
    public DateTime CreatedAt { get; set; }
    [Required]
    public DateTime UpdatedAt { get; set; }
    [Required]
    public required TKey Id { get; set; }
}