namespace DataMount.Api.Options;

public class CookieAuthOptions
{
    public long Lifetime { get; set; } = 604_800_000; // 1 week
}