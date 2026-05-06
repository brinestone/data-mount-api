using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DataMount.Api.Resources;

namespace DataMount.Api.Payloads;

public class EmailSignUpRequest
{
    [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "required_field",
         ErrorMessageResourceType = typeof(AuthMessages)), EmailAddress, DefaultValue(null), JsonPropertyName("email"),
     DisplayName("email")]
    public string Email { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "required_field",
         ErrorMessageResourceType = typeof(AuthMessages)), MinLength(8), DefaultValue(null),
     JsonPropertyName("password"), DisplayName("password")]
    public string Password { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "required_field",
        ErrorMessageResourceType = typeof(AuthMessages))]
    [Compare(nameof(Password), ErrorMessageResourceName = "password_mismatch",
         ErrorMessageResourceType = typeof(AuthMessages)), DefaultValue(null),
     JsonPropertyName("confirmPassword"), DisplayName("confirmPassword")]
    public string? ConfirmPassword { get; set; } = null;
}