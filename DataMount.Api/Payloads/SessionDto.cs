namespace DataMount.Api.Payloads;

public class SessionDto<TKey> : BaseEntityDto<TKey>
{
    // public string? UserAgent { get; set; }
    public string Ip { get; set; } = string.Empty;
    public TKey? AccountId { get; set; }
}