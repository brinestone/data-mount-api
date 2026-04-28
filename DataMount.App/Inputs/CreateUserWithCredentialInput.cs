using DataMount.Domain.Models.Identity;

namespace DataMount.App.Inputs;

public record CreateUserWithCredentialInput(
    string? FirstName,
    string LastName,
    string Password,
    string Identifier,
    ContactType ContactType
);