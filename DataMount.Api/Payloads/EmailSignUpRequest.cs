using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataMount.Api.Payloads;

public class EmailSignUpRequest
{
    [Required(AllowEmptyStrings = false), EmailAddress, DefaultValue(null), JsonPropertyName("email"), DisplayName("email")]
    public string Email { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false), MinLength(8), DefaultValue(null), JsonPropertyName("password"), DisplayName("password")]
    public string Password { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false), Compare(nameof(Password)), DefaultValue(null),
     JsonPropertyName("confirmPassword"), DisplayName("confirmPassword")]
    public string? ConfirmPassword { get; set; } = null;
}