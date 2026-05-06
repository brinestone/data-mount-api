using System.ComponentModel.DataAnnotations;
using DataMount.Api.Resources;

namespace DataMount.Api.Payloads;

public record EmailSignInRequest(
    [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "required_field",
        ErrorMessageResourceType = typeof(AuthMessages))]
    [EmailAddress(ErrorMessageResourceName = "invalid_email", ErrorMessageResourceType = typeof(AuthMessages))]
    string? Email = null,
    [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "required_field",
        ErrorMessageResourceType = typeof(AuthMessages))]
    string? Password = null,
    bool StaySignedIn = false
);