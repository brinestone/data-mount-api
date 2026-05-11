namespace DataMount.Api.Payloads;

public class ContactDto<TKey> : BaseEntityDto<TKey>
{
    public bool Verified => VerifiedAt is not null;
    public DateTime? VerifiedAt { get; set; }
    public string Type { get; set; }
    public string? Value { get; set; }
}