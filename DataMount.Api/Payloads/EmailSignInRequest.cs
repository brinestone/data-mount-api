using System.ComponentModel.DataAnnotations;

namespace DataMount.Api.Payloads;

public record EmailSignInRequest(
    [Required(AllowEmptyStrings = false), EmailAddress]
    string? Email = null,
    [Required(AllowEmptyStrings = false)] string? Password = null,
    bool StaySignedIn = false
);