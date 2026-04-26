namespace DataMount.Domain.Models.Identity;

public class CredentialAccount<TKey> : Account<TKey> where TKey : struct, IEquatable<TKey>
{
    // public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}
