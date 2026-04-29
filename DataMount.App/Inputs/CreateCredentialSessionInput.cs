using DataMount.Domain.Models.Identity;

namespace DataMount.App.Inputs;

public class CreateCredentialSessionInput
{
    public required string Identifier { get; set; }
    public required ContactType ContactType { get; set; }
    public required string Password { get; set; }
    public string? Ip { get; set; }
    public string? UserAgent { get; set; }
}