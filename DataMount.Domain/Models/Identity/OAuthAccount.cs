namespace DataMount.Domain.Models.Identity;

// public class ServiceAccount<TKey> : Account<TKey> where TKey : struct, IEquatable<TKey>
// {
//     public string ServiceName { get; set; } = string.Empty;
//     public TKey? AccessTokenId { get; set; }
// }

public class OAuthAccount<TKey> : Account<TKey> where TKey : struct, IEquatable<TKey>
{
    public string AccessToken { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public OauthProvider Provider { get; set; }
}