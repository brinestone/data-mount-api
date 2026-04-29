using System.ComponentModel.DataAnnotations;

namespace DataMount.Api.Payloads;

public record EmailSignInRequest(
    [Required(AllowEmptyStrings = false), EmailAddress]
    string Email,
    [Required(AllowEmptyStrings = false)] string Password,
    bool StaySignedIn = false
);