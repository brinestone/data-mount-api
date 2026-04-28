using System.ComponentModel.DataAnnotations;

namespace DataMount.Api.Dto;

public class EmailSignUpRequest
{
    [Required(AllowEmptyStrings = false), EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false)]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false)]
    [Compare(nameof(Password))]
    public string? ConfirmPassword { get; set; }

    public string? FirstName { get; set; }
    [Required(AllowEmptyStrings = false)] public string LastName { get; set; } = string.Empty;
}