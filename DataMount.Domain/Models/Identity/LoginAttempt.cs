namespace DataMount.Domain.Models.Identity;

public class LoginAttempt<TKey> : BaseEntity<TKey> where TKey : struct, IEquatable<TKey>
{
    public TKey? AccountId { get; set; }
    public DateTime? FailedAt { get; set; }
    public bool IsSuccess => !FailedAt.HasValue;
    public string? FailureReason { get; set; }
    public string? Ip { get; set; }
    public string? UserAgent { get; set; }
    public virtual Account<TKey>? Account { get; set; }
}