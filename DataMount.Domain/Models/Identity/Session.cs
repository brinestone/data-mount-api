namespace DataMount.Domain.Models.Identity;

public class Session<TKey> : BaseEntity<TKey> where TKey : struct, IEquatable<TKey>
{
    public string? UserAgent { get; set; }
    public string? Ip { get; set; }
    public TKey? AccountId { get; set; }
    public TKey AttemptId { get; set; }

    public virtual LoginAttempt<TKey>? Attempt { get; set; }
    public virtual Account<TKey>? Account { get; set; }
}